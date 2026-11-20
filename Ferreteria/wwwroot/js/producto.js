document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#productoTable');
    const modalElement = document.getElementById('productoModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('productoForm');
    const modalTitle = document.getElementById('productoModalTitle');
    const config = window.productosConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Producto/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'codigo' },
            { data: 'name' },
            { data: 'categoriaName' },
            { data: 'marcaName' },
            {
                data: 'precioCompra',
                render: (data) => data != null ? Number(data).toFixed(2) : '0.00'
            },
            {
                data: 'precioVenta',
                render: (data) => data != null ? Number(data).toFixed(2) : '0.00'
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
        const validator = new JustValidate('#productoForm');
        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del producto es obligatorio' }
            ])
            .addField('#CategoriaId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una categoría' }
            ])
            .addField('#MarcaId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una marca' }
            ])
            .addField('#PrecioVenta', [
                { rule: 'required', errorMessage: 'El precio de venta es obligatorio' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Producto/Update?id=${id}` : '/Producto/Create';
                const formData = new FormData(form);

                try {
                    const response = await axios.post(url, formData);
                    if (response.data.success) {
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar el producto.'
                        });
                    }
                } catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: error.response?.data?.errorMessage || error.response?.data?.message || 'No se pudo completar la operación.'
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
                if (document.getElementById('Codigo')) document.getElementById('Codigo').value = data.codigo || '';
                document.getElementById('Name').value = data.name;
                document.getElementById('CategoriaId').value = data.categoriaId;
                document.getElementById('MarcaId').value = data.marcaId;
                if (document.getElementById('PrecioCompra')) document.getElementById('PrecioCompra').value = data.precioCompra ?? '';
                document.getElementById('PrecioVenta').value = data.precioVenta ?? '';
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
            if (response.data.success) {
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el producto.'
                });
            }
        } catch (error) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error.response?.data?.errorMessage || error.response?.data?.message || 'No se pudo completar la operación.'
            });
        }
    });
});
