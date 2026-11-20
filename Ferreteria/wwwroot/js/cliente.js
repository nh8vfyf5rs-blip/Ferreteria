document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#clienteTable');
    const modalElement = document.getElementById('clienteModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('clienteForm');
    const modalTitle = document.getElementById('clienteModalTitle');
    const config = window.clientesConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Cliente/GetData',
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
            { data: 'direccion' },
            {
                data: 'fechaRegistro',
                render: (data) => data ? new Date(data).toLocaleDateString('es-NI') : ''
            },
            { data: 'totalVentas' },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-cliente me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-cliente" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#clienteForm');
        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del cliente es obligatorio' }
            ])
            .addField('#Apellido', [
                { rule: 'required', errorMessage: 'El apellido del cliente es obligatorio' }
            ])
            .addField('#Cedula', [
                { rule: 'required', errorMessage: 'La cédula del cliente es obligatoria' }
            ])
            .addField('#Correo', [
                { rule: 'email', errorMessage: 'Ingrese un correo válido' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Cliente/Update?id=${id}` : '/Cliente/Create';
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar el cliente.'
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

        document.getElementById('btnNuevoCliente')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Cliente';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-cliente', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Cliente/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Name').value = data.name;
                document.getElementById('Apellido').value = data.apellido;
                document.getElementById('Cedula').value = data.cedula;
                document.getElementById('Telefono').value = data.telefono || '';
                document.getElementById('Correo').value = data.correo || '';
                document.getElementById('Direccion').value = data.direccion || '';
                modalTitle.textContent = 'Editar Cliente';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el cliente.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-cliente', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar cliente?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Cliente/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el cliente.'
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
