document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#proveedorTable');
    const modalElement = document.getElementById('proveedorModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('proveedorForm');
    const modalTitle = document.getElementById('proveedorModalTitle');
    const config = window.proveedoresConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Proveedor/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'ruc' },
            { data: 'telefono' },
            { data: 'correo' },
            { data: 'direccion' },
            { data: 'totalCompras' },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-proveedor me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-proveedor" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#proveedorForm');
        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del proveedor es obligatorio' }
            ])
            .addField('#Ruc', [
                { rule: 'required', errorMessage: 'El RUC es obligatorio' }
            ])
            .addField('#Correo', [
                { rule: 'email', errorMessage: 'Ingrese un correo válido' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Proveedor/Update?id=${id}` : '/Proveedor/Create';
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar el proveedor.'
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

        document.getElementById('btnNuevoProveedor')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Proveedor';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-proveedor', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Proveedor/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Name').value = data.name;
                document.getElementById('Ruc').value = data.ruc || '';
                document.getElementById('Telefono').value = data.telefono || '';
                document.getElementById('Correo').value = data.correo || '';
                document.getElementById('Direccion').value = data.direccion || '';
                modalTitle.textContent = 'Editar Proveedor';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el proveedor.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-proveedor', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar proveedor?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Proveedor/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el proveedor.'
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