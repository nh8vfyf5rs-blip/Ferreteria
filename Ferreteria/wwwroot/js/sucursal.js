document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#sucursalTable');
    const modalElement = document.getElementById('sucursalModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('sucursalForm');
    const modalTitle = document.getElementById('sucursalModalTitle');
    const config = window.sucursalesConfig || { canUpdate: false, canDelete: false };

    const regexSoloDigitos = /^\d+$/;
    const nameInput = document.getElementById('Name');
    const telefonoInput = document.getElementById('Telefono');
    const direccionInput = document.getElementById('Direccion');

    const limpiarTelefono = (el) => {
        if (!el) return;
        const maxAttr = parseInt(el.getAttribute('maxlength') || '8', 10);
        const maxLen = Number.isFinite(maxAttr) && maxAttr > 0 ? maxAttr : 8;
        const limpio = String(el.value ?? '').replace(/\D/g, '').slice(0, maxLen);
        if (el.value !== limpio) el.value = limpio;
    };

    telefonoInput?.addEventListener('input', () => limpiarTelefono(telefonoInput));

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Sucursal/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'direccion' },
            { data: 'telefono' },
            { data: 'totalAlmacenes' },
            { data: 'totalEmppleados' },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-sucursal me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-sucursal" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#sucursalForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre de la sucursal es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' }
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
            .addField('#Direccion', [
                { rule: 'required', errorMessage: 'La dirección es obligatoria' },
                { rule: 'maxLength', value: 200, errorMessage: 'Máximo 200 caracteres' }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();

                limpiarTelefono(telefonoInput);
                if (nameInput) nameInput.value = nameInput.value.trim();
                if (direccionInput) direccionInput.value = direccionInput.value.trim();

                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Sucursal/Update?id=${id}` : '/Sucursal/Create';
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
                            text: response.data?.errorMessage || response.data?.message || 'No se pudo guardar la sucursal.'
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

        document.getElementById('btnNuevaSucursal')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nueva Sucursal';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-sucursal', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Sucursal/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Name').value = data.name ?? '';
                document.getElementById('Direccion').value = data.direccion ?? '';
                document.getElementById('Telefono').value = data.telefono || '';
                limpiarTelefono(telefonoInput);
                modalTitle.textContent = 'Editar Sucursal';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar la sucursal.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-sucursal', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar sucursal?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Sucursal/Delete?id=${id}`);
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
                    text: response.data?.errorMessage || response.data?.message || 'No se pudo eliminar la sucursal.'
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