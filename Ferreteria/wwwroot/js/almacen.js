document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#almacenTable');
    const modalElement = document.getElementById('almacenModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('almacenForm');
    const modalTitle = document.getElementById('almacenModalTitle');
    const config = window.almacenesConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Almacen/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'sucursalName' },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-almacen me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-almacen" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#almacenForm');
        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del almacén es obligatorio' }
            ])
            .addField('#SucursalId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una sucursal' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? '/Almacen/Update' : '/Almacen/Create';
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar el almacén.'
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

        document.getElementById('btnNuevoAlmacen')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Almacén';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-almacen', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Almacen/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Name').value = data.name;
                document.getElementById('SucursalId').value = data.sucursalId;
                modalTitle.textContent = 'Editar Almacén';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el almacén.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-almacen', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar almacén?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Almacen/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el almacén.'
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
