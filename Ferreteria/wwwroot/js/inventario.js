document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#inventarioTable');
    const modalElement = document.getElementById('inventarioModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('inventarioForm');
    const modalTitle = document.getElementById('inventarioModalTitle');
    const config = window.inventariosConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Inventario/GetData',
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
                render: function (_, __, row) {
                    return row.sucursalNombre ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.almacenNombre ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.productoNombre ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.stock ?? 0;
                }
            },
            {
                data: 'fechaActualizacion',
                render: function (value) {
                    if (!value) return '-';
                    const fecha = new Date(value);
                    return isNaN(fecha.getTime()) ? value : fecha.toLocaleDateString('es-NI');
                }
            },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-inventario me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-inventario" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#inventarioForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        if (document.getElementById('AlmacenId')) {
            validator.addField('#AlmacenId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un almacén' }
            ]);
        }

        if (document.getElementById('ProductoId')) {
            validator.addField('#ProductoId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un producto' }
            ]);
        }

        if (document.getElementById('Stock')) {
            validator.addField('#Stock', [
                { rule: 'required', errorMessage: 'El stock es obligatorio' },
                { rule: 'number', errorMessage: 'Debe ser un número válido' },
                {
                    validator: (value) => parseInt(value, 10) >= 0,
                    errorMessage: 'El stock no puede ser negativo'
                }
            ]);
        }

        validator.onSuccess(async (event) => {
            event.preventDefault();

            const idVal = document.getElementById('Id')?.value;
            const parsedId = idVal ? parseInt(idVal, 10) : 0;
            const isEdit = parsedId > 0;
            const url = isEdit ? `/Inventario/Update?id=${parsedId}` : '/Inventario/Create';

            const payload = {
                almacenId: parseInt(document.getElementById('AlmacenId').value, 10),
                productoId: parseInt(document.getElementById('ProductoId').value, 10),
                stock: parseInt(document.getElementById('Stock').value, 10)
            };

            try {
                const response = await axios.post(url, payload);
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
                        text: response.data.errorMessage || response.data.message || 'No se pudo guardar el inventario.'
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

        document.getElementById('btnNuevoInventario')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            document.getElementById('Stock').value = 0;
            modalTitle.textContent = 'Nuevo Registro de Inventario';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-inventario', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Inventario/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('AlmacenId').value = data.almacenId;
                document.getElementById('ProductoId').value = data.productoId;
                document.getElementById('Stock').value = data.stock ?? 0;
                modalTitle.textContent = 'Editar Inventario';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el registro de inventario.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-inventario', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar registro?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Inventario/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el registro.'
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