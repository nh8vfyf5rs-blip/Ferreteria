document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#categoriaTable');
    const modalElement = document.getElementById('categoriaModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('categoriaForm');
    const modalTitle = document.getElementById('categoriaModalTitle');
    const config = window.categoriasConfig || { canUpdate: false, canDelete: false };

    const regexSoloLetras = /^[A-Za-zÁÉÍÓÚÜáéíóúüÑñ\s'-]+$/;

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Categoria/GetData',
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
                data: null,
                render: function (_, __, row) {
                    return row.totalProductos ?? 0;
                }
            },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-categoria me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-categoria" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#categoriaForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        const nombreInput = document.getElementById('Nombre');
        if (nombreInput) {
            validator.addField('#Nombre', [
                { rule: 'required', errorMessage: 'El nombre de la categoría es obligatorio' },
                { rule: 'maxLength', value: 100, errorMessage: 'Máximo 100 caracteres' },
                {
                    validator: (value) => {
                        const limpio = String(value ?? '').trim();
                        if (!limpio) return true;
                        return regexSoloLetras.test(limpio);
                    },
                    errorMessage: 'El nombre solo puede contener letras'
                }
            ]);
        }

        const descInput = document.getElementById('Descripcion');
        if (descInput) {
            validator.addField('#Descripcion', [
                { rule: 'maxLength', value: 200, errorMessage: 'Máximo 200 caracteres' }
            ]);
        }

        validator.onSuccess(async (event) => {
            event.preventDefault();

            const idVal = document.getElementById('Id')?.value;
            const parsedId = idVal ? parseInt(idVal, 10) : 0;
            const isEdit = parsedId > 0;
            const url = isEdit ? '/Categoria/Update' : '/Categoria/Create';

            const payload = isEdit
                ? {
                    id: parsedId,
                    nombre: (document.getElementById('Nombre')?.value || '').trim(),
                    descripcion: (document.getElementById('Descripcion')?.value || '').trim()
                }
                : {
                    nombre: (document.getElementById('Nombre')?.value || '').trim(),
                    descripcion: (document.getElementById('Descripcion')?.value || '').trim()
                };

            try {
                const response = await axios.post(url, payload);
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
                        text: response.data?.errorMessage || response.data?.message || 'No se pudo guardar la categoría.'
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

        document.getElementById('btnNuevaCategoria')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nueva Categoría';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-categoria', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Categoria/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Nombre').value = data.nombre ?? data.name ?? '';
                if (document.getElementById('Descripcion')) {
                    document.getElementById('Descripcion').value = data.descripcion ?? '';
                }
                modalTitle.textContent = 'Editar Categoría';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar la categoría.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-categoria', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar categoría?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Categoria/Delete?id=${id}`);
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
                    text: response.data?.errorMessage || response.data?.message || 'No se pudo eliminar la categoría.'
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