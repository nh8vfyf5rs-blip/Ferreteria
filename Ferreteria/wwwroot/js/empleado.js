document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#empleadoTable');
    const modalElement = document.getElementById('empleadoModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('empleadoForm');
    const modalTitle = document.getElementById('empleadoModalTitle');
    const config = window.empleadosConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Empleado/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'apellido' },
            { data: 'cedula' },
            { data: 'telefono' },
            { data: 'correo' },
            { data: 'cargoName' },
            { data: 'sucursalName' },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-empleado me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-empleado" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#empleadoForm');
        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del empleado es obligatorio' }
            ])
            .addField('#Apellido', [
                { rule: 'required', errorMessage: 'El apellido del empleado es obligatorio' }
            ])
            .addField('#Cedula', [
                { rule: 'required', errorMessage: 'La cédula del empleado es obligatoria' }
            ])
            .addField('#CargoId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un cargo' }
            ])
            .addField('#SucursalId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una sucursal' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Empleado/Update?id=${id}` : '/Empleado/Create';
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar el empleado.'
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

        document.getElementById('btnNuevoEmpleado')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Empleado';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-empleado', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Empleado/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Name').value = data.name;
                document.getElementById('Apellido').value = data.apellido;
                document.getElementById('Cedula').value = data.cedula;
                document.getElementById('Telefono').value = data.telefono || '';
                document.getElementById('Correo').value = data.correo || '';
                document.getElementById('CargoId').value = data.cargoId;
                document.getElementById('SucursalId').value = data.sucursalId;
                modalTitle.textContent = 'Editar Empleado';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el empleado.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-empleado', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar empleado?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Empleado/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el empleado.'
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
