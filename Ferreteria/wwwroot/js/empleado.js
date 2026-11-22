document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#empleadoTable');
    const modalElement = document.getElementById('empleadoModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('empleadoForm');
    const modalTitle = document.getElementById('empleadoModalTitle');
    const config = window.empleadosConfig || { canUpdate: false, canDelete: false };

    const regexSoloLetras = /^[A-Za-zÁÉÍÓÚÜáéíóúüÑñ\s'-]+$/;
    const regexSoloDigitos = /^\d+$/;

    const nombresInput = document.getElementById('Nombres');
    const apellidosInput = document.getElementById('Apellidos');
    const telefonoInput = document.getElementById('Telefono');
    const salarioInput = document.getElementById('Salario');

    const limpiarLetras = (el) => {
        if (!el) return;
        const limpio = el.value.replace(/[^A-Za-zÁÉÍÓÚÜáéíóúüÑñ\s'-]/g, '');
        if (el.value !== limpio) el.value = limpio;
    };

    const limpiarTelefono = (el) => {
        if (!el) return;
        const maxAttr = parseInt(el.getAttribute('maxlength') || '8', 10);
        const maxLen = Number.isFinite(maxAttr) && maxAttr > 0 ? maxAttr : 8;
        const limpio = el.value.replace(/\D/g, '').slice(0, maxLen);
        if (el.value !== limpio) el.value = limpio;
    };

    const limpiarSalario = (el) => {
        if (!el) return;
        let val = String(el.value ?? '').replace(',', '.').replace(/[^0-9.]/g, '');
        const partes = val.split('.');
        if (partes.length > 2) {
            val = partes[0] + '.' + partes.slice(1).join('');
        }
        if (partes.length === 2 && partes[1].length > 2) {
            val = partes[0] + '.' + partes[1].slice(0, 2);
        }
        if (val.length > 12) {
            val = val.slice(0, 12);
        }
        if (el.value !== val) el.value = val;
    };

    nombresInput?.addEventListener('input', () => limpiarLetras(nombresInput));
    apellidosInput?.addEventListener('input', () => limpiarLetras(apellidosInput));
    telefonoInput?.addEventListener('input', () => limpiarTelefono(telefonoInput));
    salarioInput?.addEventListener('input', () => limpiarSalario(salarioInput));

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Empleado/GetData',
            type: 'POST',
            dataSrc: function (json) {
                if (Array.isArray(json)) return json;
                if (Array.isArray(json?.data)) return json.data;
                if (Array.isArray(json?.items)) return json.items;
                return [];
            }
        },
        columns: [
            { data: 'id' },
            {
                data: null,
                render: (_, __, row) => row.nombres ?? row.name ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.apellidos ?? row.apellido ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.cedula ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.telefono ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.correo ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.direccion ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => {
                    const rawDate = row.fechaContratacion;
                    if (!rawDate) return '-';
                    const dt = new Date(rawDate);
                    return isNaN(dt.getTime()) ? rawDate : dt.toLocaleDateString('es-NI');
                }
            },
            {
                data: null,
                render: (_, __, row) => {
                    const val = row.salario ?? 0;
                    return Number(val).toFixed(2);
                }
            },
            {
                data: null,
                render: (_, __, row) => row.cargoNombre ?? row.cargoName ?? '-'
            },
            {
                data: null,
                render: (_, __, row) => row.sucursalNombre ?? row.sucursalName ?? '-'
            },
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
        const validator = new JustValidate('#empleadoForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        validator
            .addField('#Nombres', [
                { rule: 'required', errorMessage: 'El nombre del empleado es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' },
                {
                    validator: (value) => {
                        const limpio = String(value ?? '').trim();
                        if (!limpio) return true;
                        return regexSoloLetras.test(limpio);
                    },
                    errorMessage: 'Los nombres solo pueden contener letras'
                }
            ])
            .addField('#Apellidos', [
                { rule: 'required', errorMessage: 'El apellido del empleado es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' },
                {
                    validator: (value) => {
                        const limpio = String(value ?? '').trim();
                        if (!limpio) return true;
                        return regexSoloLetras.test(limpio);
                    },
                    errorMessage: 'Los apellidos solo pueden contener letras'
                }
            ])
            .addField('#Cedula', [
                { rule: 'required', errorMessage: 'La cédula del empleado es obligatoria' },
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
            .addField('#CargoId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un cargo' }
            ])
            .addField('#SucursalId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una sucursal' }
            ])
            .addField('#FechaContratacion', [
                { rule: 'required', errorMessage: 'La fecha de contratación es obligatoria' }
            ])
            .addField('#Salario', [
                { rule: 'required', errorMessage: 'El salario es obligatorio' },
                {
                    validator: (value) => {
                        const num = Number(String(value ?? '').trim().replace(',', '.'));
                        return Number.isFinite(num) && num >= 0;
                    },
                    errorMessage: 'Ingrese un salario válido mayor o igual a 0'
                }
            ])
            .addField('#Direccion', [
                { rule: 'maxLength', value: 200, errorMessage: 'Máximo 200 caracteres' }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();

                limpiarLetras(nombresInput);
                limpiarLetras(apellidosInput);
                limpiarTelefono(telefonoInput);
                limpiarSalario(salarioInput);

                if (nombresInput) nombresInput.value = nombresInput.value.trim();
                if (apellidosInput) apellidosInput.value = apellidosInput.value.trim();

                const cedulaEl = document.getElementById('Cedula');
                const correoEl = document.getElementById('Correo');
                const direccionEl = document.getElementById('Direccion');
                if (cedulaEl) cedulaEl.value = cedulaEl.value.trim();
                if (correoEl) correoEl.value = correoEl.value.trim();
                if (direccionEl) direccionEl.value = direccionEl.value.trim();

                const idVal = document.getElementById('Id')?.value;
                const parsedId = idVal ? parseInt(idVal, 10) : 0;
                const isEdit = parsedId > 0;
                const url = isEdit ? `/Empleado/Update?id=${parsedId}` : '/Empleado/Create';
                const formData = new FormData(form);

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
                            text: response.data?.errorMessage || response.data?.message || 'No se pudo guardar el empleado.'
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
            const fechaInput = document.getElementById('FechaContratacion');
            if (fechaInput && !fechaInput.value) {
                fechaInput.value = new Date().toISOString().split('T')[0];
            }
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

                document.getElementById('Id').value = data.id ?? '';
                document.getElementById('Nombres').value = data.nombres ?? data.name ?? '';
                document.getElementById('Apellidos').value = data.apellidos ?? data.apellido ?? '';
                document.getElementById('Cedula').value = data.cedula ?? '';
                document.getElementById('Telefono').value = data.telefono ?? '';
                document.getElementById('Correo').value = data.correo ?? '';
                document.getElementById('Direccion').value = data.direccion ?? '';
                document.getElementById('Salario').value = data.salario ?? '';
                document.getElementById('CargoId').value = data.cargoId ?? '';
                document.getElementById('SucursalId').value = data.sucursalId ?? '';

                limpiarLetras(nombresInput);
                limpiarLetras(apellidosInput);
                limpiarTelefono(telefonoInput);
                limpiarSalario(salarioInput);

                if (data.fechaContratacion) {
                    const dt = new Date(data.fechaContratacion);
                    if (!isNaN(dt.getTime())) {
                        document.getElementById('FechaContratacion').value = dt.toISOString().split('T')[0];
                    }
                }

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
                    text: response.data?.errorMessage || response.data?.message || 'No se pudo eliminar el empleado.'
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