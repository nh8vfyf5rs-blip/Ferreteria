document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#clienteTable');
    if (!tableElement.length) return;

    const modalElement = document.getElementById('clienteModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('clienteForm');
    const modalTitle = document.getElementById('clienteModalTitle');
    const config = window.clientesConfig || { canUpdate: false, canDelete: false };

    const regexSoloLetras = /^[A-Za-zÁÉÍÓÚÜáéíóúüÑñ\s'-]+$/;
    const regexSoloDigitos = /^\d+$/;

    const telefonoInput = document.getElementById('Telefono');
    const aplicarFiltroTelefono = () => {
        if (!telefonoInput) return;
        const maxAttr = parseInt(telefonoInput.getAttribute('maxlength') || '8', 10);
        const maxLen = Number.isFinite(maxAttr) && maxAttr > 0 ? maxAttr : 8;
        const limpio = telefonoInput.value.replace(/\D/g, '').slice(0, maxLen);
        if (telefonoInput.value !== limpio) {
            telefonoInput.value = limpio;
        }
    };

    telefonoInput?.addEventListener('input', aplicarFiltroTelefono);
    telefonoInput?.addEventListener('change', aplicarFiltroTelefono);

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
        const validator = new JustValidate('#clienteForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del cliente es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' },
                {
                    validator: (value) => {
                        const limpio = String(value ?? '').trim();
                        if (!limpio) return true;
                        return regexSoloLetras.test(limpio);
                    },
                    errorMessage: 'El nombre solo puede contener letras'
                }
            ])
            .addField('#Apellido', [
                { rule: 'required', errorMessage: 'El apellido del cliente es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' },
                {
                    validator: (value) => {
                        const limpio = String(value ?? '').trim();
                        if (!limpio) return true;
                        return regexSoloLetras.test(limpio);
                    },
                    errorMessage: 'El apellido solo puede contener letras'
                }
            ])
            .addField('#Cedula', [
                { rule: 'required', errorMessage: 'La cédula del cliente es obligatoria' },
                { rule: 'maxLength', value: 30, errorMessage: 'Máximo 30 caracteres' }
            ])
            .addField('#Telefono', [
                { rule: 'maxLength', value: 8, errorMessage: 'Máximo 8 dígitos' },
                {
                    validator: (value) => {
                        const limpio = String(value ?? '').trim();
                        if (!limpio) return true;
                        return regexSoloDigitos.test(limpio) && limpio.length <= 8;
                    },
                    errorMessage: 'El teléfono solo puede contener hasta 8 dígitos'
                }
            ])
            .addField('#Correo', [
                { rule: 'maxLength', value: 120, errorMessage: 'Máximo 120 caracteres' },
                { rule: 'email', errorMessage: 'Ingrese un correo válido' }
            ])
            .addField('#Direccion', [
                { rule: 'maxLength', value: 200, errorMessage: 'Máximo 200 caracteres' }
            ])
            .onSuccess(async () => {
                aplicarFiltroTelefono();

                const nameEl = document.getElementById('Name');
                const apellidoEl = document.getElementById('Apellido');
                const cedulaEl = document.getElementById('Cedula');
                const correoEl = document.getElementById('Correo');
                const direccionEl = document.getElementById('Direccion');

                if (nameEl) nameEl.value = nameEl.value.trim();
                if (apellidoEl) apellidoEl.value = apellidoEl.value.trim();
                if (cedulaEl) cedulaEl.value = cedulaEl.value.trim();
                if (correoEl) correoEl.value = correoEl.value.trim();
                if (direccionEl) direccionEl.value = direccionEl.value.trim();

                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Cliente/Update?id=${id}` : '/Cliente/Create';
                const formData = new FormData(form);

                if (!isEdit) {
                    formData.delete('Id');
                }

                try {
                    const response = await axios.post(url, formData);
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
                            text: response.data?.errorMessage || response.data?.message || 'No se pudo guardar el cliente.'
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
                document.getElementById('Name').value = data.name ?? '';
                document.getElementById('Apellido').value = data.apellido ?? '';
                document.getElementById('Cedula').value = data.cedula ?? '';
                document.getElementById('Telefono').value = data.telefono ?? '';
                aplicarFiltroTelefono();
                document.getElementById('Correo').value = data.correo ?? '';
                document.getElementById('Direccion').value = data.direccion ?? '';
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
                    text: response.data?.errorMessage || response.data?.message || 'No se pudo eliminar el cliente.'
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