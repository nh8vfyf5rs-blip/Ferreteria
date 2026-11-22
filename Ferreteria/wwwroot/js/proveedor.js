$(function () {
    const config = window.proveedoresConfig || {};
    const modalEl = document.getElementById('proveedorModal');
    const modal = (modalEl && window.bootstrap) ? new bootstrap.Modal(modalEl) : null;
    let isEdit = false;

    const formEl = document.getElementById('proveedorForm');
    let proveedorValidator = null;

    if (formEl && window.JustValidate) {
        proveedorValidator = new JustValidate('#proveedorForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        proveedorValidator
            .addField('#proveedorNombreEmpresa', [
                { rule: 'required', errorMessage: 'El nombre de la empresa es obligatorio.' },
                { rule: 'minLength', value: 2, errorMessage: 'Mínimo 2 caracteres.' },
                { rule: 'maxLength', value: 150, errorMessage: 'Máximo 150 caracteres.' }
            ])
            .addField('#proveedorContacto', [
                { rule: 'required', errorMessage: 'El contacto es obligatorio.' },
                { rule: 'minLength', value: 2, errorMessage: 'Mínimo 2 caracteres.' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres.' }
            ])
            .addField('#proveedorCorreo', [
                { rule: 'required', errorMessage: 'El correo es obligatorio.' },
                { rule: 'email', errorMessage: 'Ingrese un correo válido.' },
                { rule: 'maxLength', value: 150, errorMessage: 'Máximo 150 caracteres.' }
            ])
            .addField('#proveedorDireccion', [
                { rule: 'required', errorMessage: 'La dirección es obligatoria.' },
                { rule: 'maxLength', value: 200, errorMessage: 'Máximo 200 caracteres.' }
            ]);

        proveedorValidator.onSuccess(async (event) => {
            event.preventDefault();

            const idVal = $('#proveedorId').val();
            const parsedId = idVal ? parseInt(idVal, 10) : 0;

            const payload = {
                nombreEmpresa: $('#proveedorNombreEmpresa').val().trim(),
                contacto: $('#proveedorContacto').val().trim(),
                correo: $('#proveedorCorreo').val().trim(),
                direccion: $('#proveedorDireccion').val().trim()
            };

            try {
                const url = isEdit
                    ? `/Proveedor/Update?id=${parsedId}`
                    : '/Proveedor/Create';

                const response = await axios.post(url, payload);

                if (response.data.success) {
                    modal?.hide();
                    table.ajax.reload(null, false);
                    Swal.fire({
                        title: 'Listo',
                        text: response.data.message,
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: false
                    });
                } else {
                    Swal.fire('Error', response.data.errorMessage || response.data.message || 'No se pudo guardar.', 'error');
                }
            } catch (error) {
                const message = error.response?.data?.errorMessage || error.response?.data?.message || 'No se pudo guardar.';
                Swal.fire('Error', message, 'error');
            }
        });
    }

    const table = $('#proveedorTable').DataTable({
        ajax: {
            url: '/Proveedor/GetData',
            type: 'POST',
            dataSrc: function (json) {
                if (Array.isArray(json)) return json;
                if (Array.isArray(json?.data)) return json.data;
                return [];
            }
        },
        columns: [
            { data: 'id' },
            { data: 'nombreEmpresa' },
            { data: 'contacto' },
            { data: 'correo' },
            { data: 'direccion' },
            {
                data: 'fechaRegistro',
                render: function (value) {
                    if (!value) return '-';
                    const fecha = new Date(value);
                    return isNaN(fecha.getTime()) ? value : fecha.toLocaleDateString('es-NI');
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.totalCompras ?? 0;
                }
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button class="btn btn-sm btn-outline-primary me-1 btn-edit" data-id="${data.id}" title="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button class="btn btn-sm btn-outline-danger btn-delete" data-id="${data.id}" data-nombre="${data.nombreEmpresa}" title="Eliminar">
                                        <i class="fa-solid fa-trash"></i>
                                    </button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>';
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[1, 'asc']]
    });

    function clearForm() {
        $('#proveedorId').val('');
        $('#proveedorNombreEmpresa').val('');
        $('#proveedorContacto').val('');
        $('#proveedorCorreo').val('');
        $('#proveedorDireccion').val('');
        proveedorValidator?.refresh();
    }

    $('#btnNuevoProveedor').on('click', function () {
        isEdit = false;
        clearForm();
        $('#proveedorModalTitle').text('Nuevo Proveedor');
        modal?.show();
    });

    $('#proveedorTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.get(`/Proveedor/GetById?id=${id}`);
            const data = response.data;
            isEdit = true;
            $('#proveedorId').val(data.id);
            $('#proveedorNombreEmpresa').val(data.nombreEmpresa);
            $('#proveedorContacto').val(data.contacto);
            $('#proveedorCorreo').val(data.correo);
            $('#proveedorDireccion').val(data.direccion);
            $('#proveedorModalTitle').text('Editar Proveedor');
            proveedorValidator?.refresh();
            modal?.show();
        } catch (error) {
            Swal.fire('Error', 'No se pudo cargar el proveedor', 'error');
        }
    });

    $('#proveedorTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eliminar proveedor?',
            html: `¿Seguro que deseas eliminar a <strong>${nombre}</strong>?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d'
        });

        if (!confirm.isConfirmed) return;

        try {
            const response = await axios.post(`/Proveedor/Delete?id=${id}`);
            if (response.data.success) {
                table.ajax.reload(null, false);
                Swal.fire('Eliminado', response.data.message, 'success');
            } else {
                Swal.fire('Error', response.data.errorMessage || response.data.message || 'No se pudo eliminar', 'error');
            }
        } catch (error) {
            const message = error.response?.data?.errorMessage || error.response?.data?.message || 'No se pudo eliminar';
            Swal.fire('Error', message, 'error');
        }
    });
});