document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#ventaTable');
    const modalElement = document.getElementById('ventaModal');
    const modal = modalElement ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('ventaForm');
    const modalTitle = document.getElementById('ventaModalTitle');
    const config = window.ventasConfig || { canUpdate: false, canDelete: false };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Venta/GetData',
            type: 'POST',
            dataSrc: ''
        },
        columns: [
            { data: 'id' },
            {
                data: 'fecha',
                render: (data) => data ? new Date(data).toLocaleDateString('es-NI') : ''
            },
            { data: 'clienteName' },
            { data: 'empleadoName' },
            { data: 'sucursalName' },
            {
                data: 'total',
                render: (data) => data != null ? Number(data).toFixed(2) : '0.00'
            },
            {
                data: 'id',
                orderable: false,
                searchable: false,
                render: (id) => {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-warning btn-editar-venta me-1" data-id="${id}" title="Editar"><i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-venta" data-id="${id}" title="Eliminar"><i class="fa-solid fa-trash"></i></button>`;
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
        const validator = new JustValidate('#ventaForm');
        validator
            .addField('#ClienteId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un cliente' }
            ])
            .addField('#SucursalId', [
                { rule: 'required', errorMessage: 'Debe seleccionar una sucursal' }
            ])
            .onSuccess(async () => {
                const id = document.getElementById('Id').value;
                const isEdit = id && parseInt(id, 10) > 0;
                const url = isEdit ? `/Venta/Update?id=${id}` : '/Venta/Create';
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
                            text: response.data.errorMessage || response.data.message || 'No se pudo guardar la venta.'
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

        document.getElementById('btnNuevaVenta')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            modalTitle.textContent = 'Nueva Venta';
            modal.show();
        });

        tableElement.on('click', '.btn-editar-venta', async function () {
            const id = $(this).data('id');
            try {
                const response = await axios.get(`/Venta/GetById?id=${id}`);
                const data = response.data;
                form.reset();
                validator.refresh();
                document.getElementById('Id').value = data.id;
                document.getElementById('ClienteId').value = data.clienteId;
                if (document.getElementById('EmpleadoId')) document.getElementById('EmpleadoId').value = data.empleadoId || '';
                document.getElementById('SucursalId').value = data.sucursalId;
                modalTitle.textContent = 'Editar Venta';
                modal.show();
            } catch {
                Swal.fire({ icon: 'error', title: 'Error', text: 'No se pudo cargar la venta.' });
            }
        });
    }

    tableElement.on('click', '.btn-eliminar-venta', async function () {
        const id = $(this).data('id');
        const confirmResult = await Swal.fire({
            icon: 'warning',
            title: '¿Eliminar venta?',
            text: 'Esta acción no se puede deshacer.',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        });

        if (!confirmResult.isConfirmed) return;

        try {
            const response = await axios.post(`/Venta/Delete?id=${id}`);
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
                    text: response.data.errorMessage || response.data.message || 'No se pudo eliminar la venta.'
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
