document.addEventListener('DOMContentLoaded', () => {
    const tableElement = $('#ventaTable');
    const modalElement = document.getElementById('ventaModal');
    const modal = (modalElement && window.bootstrap) ? new bootstrap.Modal(modalElement) : null;
    const form = document.getElementById('ventaForm');
    const modalTitle = document.getElementById('ventaModalTitle');
    const config = window.ventasConfig || { canUpdate: false, canDelete: false };

    let detalles = [];

    const tbodyDetalle = document.getElementById('tbodyDetalleVenta');
    const lblTotalVenta = document.getElementById('lblTotalVenta');
    const totalInput = document.getElementById('Total');
    const metodoPagoSelect = document.getElementById('MetodoPagoId');
    const grupoComprobante = document.getElementById('grupoComprobante');
    const comprobanteInput = document.getElementById('NumeroComprobante');

    const toSafeInt = (val) => {
        const n = parseInt(val ?? '0', 10);
        return Number.isNaN(n) ? 0 : n;
    };

    const toSafeDecimal = (val) => {
        const n = Number(val);
        return Number.isFinite(n) ? Number(n.toFixed(2)) : 0;
    };

    const formatLocalDateTime = (raw) => {
        if (raw && typeof raw === 'string' && raw.trim() !== '') {
            const clean = raw.trim();
            return clean.length === 16 ? `${clean}:00` : clean;
        }
        const now = new Date();
        const local = new Date(now.getTime() - now.getTimezoneOffset() * 60000);
        return local.toISOString().slice(0, 19);
    };

    const toggleComprobante = () => {
        const textoMetodo = (metodoPagoSelect?.options[metodoPagoSelect.selectedIndex]?.text || '').toLowerCase();
        const requiereRef = textoMetodo.includes('tarjeta') || textoMetodo.includes('transferencia') || textoMetodo.includes('banco');
        if (grupoComprobante) {
            grupoComprobante.style.display = requiereRef ? 'block' : 'none';
        }
        if (!requiereRef && comprobanteInput) {
            comprobanteInput.value = '';
        }
    };

    const renderDetalles = () => {
        if (!tbodyDetalle) return;
        tbodyDetalle.innerHTML = '';

        let total = 0;
        detalles.forEach((item, index) => {
            const subtotal = toSafeDecimal(item.cantidad * item.precioUnitario);
            total = toSafeDecimal(total + subtotal);

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${item.productoId}</td>
                <td>${item.productoNombre}</td>
                <td>${item.cantidad}</td>
                <td>${Number(item.precioUnitario).toFixed(2)}</td>
                <td>${subtotal.toFixed(2)}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-danger btn-quitar-detalle" data-index="${index}" title="Quitar">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </td>
            `;
            tbodyDetalle.appendChild(tr);
        });

        if (lblTotalVenta) lblTotalVenta.textContent = total.toFixed(2);
        if (totalInput) totalInput.value = total.toFixed(2);
    };

    const dataTable = tableElement.DataTable({
        ajax: {
            url: '/Venta/GetData',
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
                render: function (_, __, row) {
                    const rawDate = row.fechaVenta ?? row.fecha ?? '';
                    return rawDate ? new Date(rawDate).toLocaleDateString('es-NI') : '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.clienteName ?? row.clienteNombre ?? row.cliente ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.empleadoName ?? row.empleadoNombre ?? row.empleado ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.almacenName ?? row.almacenNombre ?? row.almacen ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.sucursalName ?? row.sucursalNombre ?? row.sucursal ?? '-';
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    return row.cantProductos ?? row.cantidadProductos ?? row.totalProductos ?? 0;
                }
            },
            {
                data: null,
                render: function (_, __, row) {
                    const val = row.total ?? 0;
                    return Number(val).toFixed(2);
                }
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
        const productoSelect = document.getElementById('ProductoId');
        const cantidadInput = document.getElementById('Cantidad');
        const precioInput = document.getElementById('PrecioUnitario');

        cantidadInput?.addEventListener('input', () => {
            const limpio = cantidadInput.value.replace(/\D/g, '');
            if (cantidadInput.value !== limpio) cantidadInput.value = limpio;
        });

        precioInput?.addEventListener('input', () => {
            let val = String(precioInput.value ?? '').replace(',', '.').replace(/[^0-9.]/g, '');
            const partes = val.split('.');
            if (partes.length > 2) val = partes[0] + '.' + partes.slice(1).join('');
            if (partes.length === 2 && partes[1].length > 2) val = partes[0] + '.' + partes[1].slice(0, 2);
            if (precioInput.value !== val) precioInput.value = val;
        });

        metodoPagoSelect?.addEventListener('change', toggleComprobante);

        productoSelect?.addEventListener('change', () => {
            const selectedOption = productoSelect.options[productoSelect.selectedIndex];
            const precioAttr = selectedOption?.getAttribute('data-precio');
            if (precioAttr && precioInput) {
                const parsedPrecio = parseFloat(precioAttr);
                if (!isNaN(parsedPrecio) && parsedPrecio > 0) {
                    precioInput.value = parsedPrecio;
                }
            }
        });

        document.getElementById('btnAgregarDetalle')?.addEventListener('click', () => {
            const productoId = toSafeInt(productoSelect?.value);
            const productoNombre = productoSelect?.options[productoSelect.selectedIndex]?.text || '';
            const cantidad = toSafeInt(cantidadInput?.value);
            const precioUnitario = toSafeDecimal(precioInput?.value);

            if (!productoId || cantidad <= 0 || precioUnitario <= 0) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Datos incompletos',
                    text: 'Selecciona un producto, una cantidad mayor a 0 y un precio válido.'
                });
                return;
            }

            const existente = detalles.find(d => d.productoId === productoId);
            if (existente) {
                existente.cantidad += cantidad;
                existente.precioUnitario = precioUnitario;
            } else {
                detalles.push({
                    productoId,
                    productoNombre,
                    cantidad,
                    precioUnitario
                });
            }

            renderDetalles();
            productoSelect.value = '';
            cantidadInput.value = '';
            precioInput.value = '';
        });

        tbodyDetalle?.addEventListener('click', (e) => {
            const btn = e.target.closest('.btn-quitar-detalle');
            if (!btn) return;
            const index = parseInt(btn.getAttribute('data-index') || '-1', 10);
            if (index >= 0) {
                detalles.splice(index, 1);
                renderDetalles();
            }
        });

        const validator = new JustValidate('#ventaForm', {
            validateBeforeSubmitting: true,
            focusInvalidField: true,
            errorFieldCssClass: 'is-invalid',
            successFieldCssClass: 'is-valid'
        });

        if (document.getElementById('ClienteId')) {
            validator.addField('#ClienteId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un cliente' }
            ]);
        }

        if (document.getElementById('EmpleadoId')) {
            validator.addField('#EmpleadoId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un empleado' }
            ]);
        }

        if (document.getElementById('AlmacenId')) {
            validator.addField('#AlmacenId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un almacén' }
            ]);
        }

        if (document.getElementById('MetodoPagoId')) {
            validator.addField('#MetodoPagoId', [
                { rule: 'required', errorMessage: 'Debe seleccionar un método de pago' }
            ]);
        }

        validator.onSuccess(async (event) => {
            event.preventDefault();

            if (detalles.length === 0) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Detalle vacío',
                    text: 'Agrega al menos un producto a la venta.'
                });
                return;
            }

            const idVal = document.getElementById('Id')?.value;
            const parsedId = idVal ? toSafeInt(idVal) : 0;
            const isEdit = parsedId > 0;
            const url = isEdit ? `/Venta/Update?id=${parsedId}` : '/Venta/Create';

            const totalCalculado = toSafeDecimal(
                detalles.reduce((acc, d) => acc + (toSafeInt(d.cantidad) * toSafeDecimal(d.precioUnitario)), 0)
            );

            const payload = {
                ...(isEdit ? { id: parsedId } : {}),
                clienteId: toSafeInt(document.getElementById('ClienteId')?.value),
                empleadoId: toSafeInt(document.getElementById('EmpleadoId')?.value),
                almacenId: toSafeInt(document.getElementById('AlmacenId')?.value),
                metodoPagoId: toSafeInt(metodoPagoSelect?.value),
                numeroComprobante: (comprobanteInput?.value || '').trim() || null,
                fechaVenta: formatLocalDateTime(document.getElementById('FechaVenta')?.value),
                total: totalCalculado,
                detalleVentaDtos: detalles.map(d => {
                    const cant = toSafeInt(d.cantidad);
                    const precio = toSafeDecimal(d.precioUnitario);
                    return {
                        productoId: toSafeInt(d.productoId),
                        cantidad: cant,
                        precioUnitario: precio,
                        subtotal: toSafeDecimal(cant * precio)
                    };
                })
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
                        text: response.data?.errorMessage || response.data?.message || 'No se pudo guardar la venta.'
                    });
                }
            } catch (error) {
                const data = error.response?.data;
                const msg = (typeof data === 'string' && data.trim())
                    ? data
                    : (data?.errorMessage || data?.message || error.message || 'No se pudo completar la operación.');
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: msg
                });
            }
        });

        document.getElementById('btnNuevaVenta')?.addEventListener('click', () => {
            form.reset();
            validator.refresh();
            document.getElementById('Id').value = '';
            if (comprobanteInput) comprobanteInput.value = '';
            toggleComprobante();
            detalles = [];
            renderDetalles();
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
                document.getElementById('ClienteId').value = data.clienteId ?? '';
                if (document.getElementById('EmpleadoId')) {
                    document.getElementById('EmpleadoId').value = data.empleadoId ?? '';
                }
                if (document.getElementById('AlmacenId')) {
                    document.getElementById('AlmacenId').value = data.almacenId ?? '';
                }
                if (metodoPagoSelect) {
                    metodoPagoSelect.value = data.metodoPagoId ?? '';
                }
                if (comprobanteInput) {
                    comprobanteInput.value = data.numeroComprobante ?? '';
                }
                toggleComprobante();

                const rawFecha = data.fechaVenta ?? data.fecha;
                if (rawFecha && document.getElementById('FechaVenta')) {
                    const dt = new Date(rawFecha);
                    const localIso = new Date(dt.getTime() - dt.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
                    document.getElementById('FechaVenta').value = localIso;
                }

                const rawDetalles = data.detalleVentaDtos ?? data.detalles ?? data.items ?? [];
                detalles = rawDetalles.map(d => ({
                    productoId: d.productoId,
                    productoNombre: d.productoNombre ?? d.productoName ?? `Producto #${d.productoId}`,
                    cantidad: Number(d.cantidad ?? 1),
                    precioUnitario: Number(d.precioUnitario ?? d.precio ?? 0)
                }));
                renderDetalles();

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
                    text: response.data?.errorMessage || response.data?.message || 'No se pudo eliminar la venta.'
                });
            }
        } catch (error) {
            const data = error.response?.data;
            const msg = (typeof data === 'string' && data.trim())
                ? data
                : (data?.errorMessage || data?.message || error.message || 'No se pudo completar la operación.');
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: msg
            });
        }
    });
});