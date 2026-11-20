document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#cargoTable');
    const modalElement = document.getElementById('cargoModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('cargoForm');
    const modalTitle = document.getElementById('cargoModalTitle');
    const config = window.cargosConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Cargo/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'descripcion' },
            { data: 'totalEmpleados' },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-cargo me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-cargo" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#cargoForm');
        validator
            .addField('#Name', [
                { rule: 'required', errorMessage: 'El nombre del cargo es obligatorio' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Cargo/Update?id=${id}` : '/Cargo/Create';
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar el cargo.'
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

        document.getElementById('btnNuevoCargo')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nuevo Cargo';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-cargo', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Cargo/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('Name').value = data.name;
                document.getElementById('Descripcion').value = data.descripcion || '';
                modalTitle.textContent = 'Editar Cargo';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar el cargo.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-cargo', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar cargo?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Cargo/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar el cargo.'
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
