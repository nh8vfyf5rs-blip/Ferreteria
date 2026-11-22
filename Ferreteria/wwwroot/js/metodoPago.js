document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#metodoPagoTable');
    const modalElement = document.getElementById('metodoPagoModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('metodoPagoForm');
    const modalTitle = document.getElementById('metodoPagoModalTitle');
    const config = window.metodosPagoConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/MetodoPago/GetData',
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
                    return row.name ?? row.nombre ?? '-';
                }
            },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-metodopago me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-metodopago" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#metodoPagoForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        const nombreInput = document.getElementById('Nombre');
        if (nombreInput) {
            validator.addField('#Nombre', [
                { rule: 'required', errorMessage: 'El nombre del método de pago es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' }
            ]);
        }

        const descripcionInput = document.getElementById('Descripcion');
        if (descripcionInput) {
            validator.addField('#Descripcion', [
                { rule: 'maxLength', value: 200, errorMessage: 'Máximo 200 caracteres' }
            ]);
        }

        validator.onSuccess(async (event) => {
            event.preventDefault();

            const idVal = document.getElementById('Id')?.value;
            const parsedId = idVal ? parseInt(idVal, 10) : 0;
            const isEdit = parsedId > 0;
            const url = isEdit ? `/MetodoPago/Update?id=${parsedId}` : '/MetodoPago/Create';

            const payload = {
                nombre: (document.getElementById('Nombre')?.value || '').trim(),
                descripcion: (document.getElementById('Descripcion')?.value || '').trim()
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
                        text: response.data.errorMessage || response.data.message || 'No se pudo guardar el método de pago.'
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

        document.getElementById('btnNuevoMetodoPago')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Método de Pago';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-metodopago', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/MetodoPago/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Nombre').value = data.nombre ?? data.name ?? '';
                if (document.getElementById('Descripcion')) {
                    document.getElementById('Descripcion').value = data.descripcion ?? '';
                }
                modalTitle.textContent = 'Editar Método de Pago';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el método de pago.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-metodopago', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar método de pago?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/MetodoPago/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el método de pago.'
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