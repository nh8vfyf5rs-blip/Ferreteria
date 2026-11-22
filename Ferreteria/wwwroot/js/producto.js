document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#productoTable');
    if (!tableElement.length) return;

    const modalElement = document.getElementById('productoModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('productoForm');
    const modalTitle = document.getElementById('productoModalTitle');
    const config = window.productosConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Producto/GetData',
            type: 'POST',
            dataSrc: function (json) {
                if (Array.isArray(json)) return json;
                if (Array.isArray(json?.data)) return json.data;
                return [];
            }
        },
        columns: [
            { data: 'id' },
            {
                data: null,
                render: (_, __, row) => row.codigo ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.nombre ?? row.name ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.categoriaNombre ?? row.categoriaName ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.marcaNombre ?? row.marcaName ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => {
                    const val = row.precioCompra ?? row.precio;
                    return val != null ? Number(val).toFixed(2) : '0.00';
                }
            },
            {
                data: null,
                render: (_, __, row) => {
                    const val = row.precioVenta ?? row.precio;
                    return val != null ? Number(val).toFixed(2) : '0.00';
                }
            },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-producto me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-producto" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || `<span class="text-muted small">Sin acciones</span>`;
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        }
    });

    if (form && modal) {
        const validator = new JustValidate('#productoForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        validator
            .addField('#Nombre', [
                { rule: 'required', errorMessage: 'El nombre del producto es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' }
            ])
            .addField('#Precio', [
                { rule: 'required', errorMessage: 'El precio es obligatorio' },
                {
                    validator: (value) => {
                        const num = Number(String(value ?? '').trim().replace(',', '.'));
                        return Number.isFinite(num) && num > 0;
                    },
                    errorMessage: 'El precio debe ser mayor a 0'
                }
            ])
            .addField('#Stock', [
                { rule: 'required', errorMessage: 'El stock es obligatorio' },
                {
                    validator: (value) => {
                        const num = Number(String(value ?? '').trim());
                        return Number.isInteger(num) && num >= 0;
                    },
                    errorMessage: 'El stock debe ser un número entero de 0 o mayor'
                }
            ])
            .addField('#CategoriaId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una categoría' }
            ])
            .addField('#ProveedorId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un proveedor' }
            ])
            .addField('#Descripcion', [
                { rule: 'maxLength', value: 250, errorMessage: 'Máximo 250 caracteres' }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();

                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Producto/Update?id=${id}` : '/Producto/Create';

                const nombreLimpio = (document.getElementById('Nombre')?.value || '').trim();
                const precioLimpio = (document.getElementById('Precio')?.value || '').trim().replace(',', '.');
                const stockLimpio = (document.getElementById('Stock')?.value || '').trim();
                const descLimpia = (document.getElementById('Descripcion')?.value || '').trim();
                const categoriaId = parseInt(document.getElementById('CategoriaId')?.value || '0', 10);
                const proveedorId = parseInt(document.getElementById('ProveedorId')?.value || '0', 10);

                const payload = {
                    Id: isEdit ? parseInt(id, 10) : 0,
                    Nombre: nombreLimpio,
                    Descripcion: descLimpia,
                    Precio: Number(precioLimpio),
                    Stock: parseInt(stockLimpio, 10),
                    CategoriaId: categoriaId,
                    ProveedorId: proveedorId
                };

                try {
                    const response = await axios.post(url, payload);
                    if (response.data?.success) {
                        modal.hide();
                        await Swal.fire({
                            icon: 'success',
                            title: 'Listo',
                            text: response.data.message,
                            timer: 1500,
                            showConfirmButton: false
                        });
                        dataTable.ajax.reload(null, false);
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: response.data?.errorMessage || response.data?.message || 'No se pudo guardar el producto.'
                        });
                    }
                } catch (error) {
                    const data = error.response?.data;
                    const mensajeError =
                        data?.errorMessage ||
                        data?.message ||
                        data?.title ||
                        'No se pudo completar la operación.';

                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: mensajeError
                    });
                }
            });

        document.getElementById('btnNuevoProducto')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Producto';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-producto', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Producto/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Nombre').value = data.nombre ?? '';
                document.getElementById('Descripcion').value = data.descripcion ?? '';
                document.getElementById('Precio').value = data.precio ?? '';
                document.getElementById('Stock').value = data.stock ?? 0;
                document.getElementById('CategoriaId').value = data.categoriaId ?? '';
                document.getElementById('ProveedorId').value = data.proveedorId ?? '';
                modalTitle.textContent = 'Editar Producto';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el producto.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-producto', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar producto?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Producto/Delete?id=${id}`);
            if (response.data?.success) {
                await Swal.fire({
                    icon: 'success',
                    title: 'Listo',
                    text: response.data.message,
                    timer: 1500,
                    showConfirmButton: false
                });
                dataTable.ajax.reload(null, false);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.data?.errorMessage || response.data?.message || 'No se pudo eliminar el producto.'
                });
            }
        } catch (error) {
            const data = error.response?.data;
            const mensajeError =
                data?.errorMessage ||
                data?.message ||
                data?.title ||
                'No se pudo completar la operación.';

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: mensajeError
            });
        }
    });
});