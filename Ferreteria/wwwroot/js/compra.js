$(function () {
    const config = window.comprasConfig || { canCreate: false, canUpdate: false, canDelete: false };
    const modalEl = document.getElementById('compraModal');
    const modal = modalEl ? new bootstrap.Modal(modalEl) : null;
    let isEdit = false;
    let detallesCompra = [];

    const toSafeInt = (val) => {
        const n = parseInt(String(val ?? '0').replace(/\D/g, ''), 10);
        return Number.isNaN(n) ? 0 : n;
    };

    const toSafeDecimal = (val) => {
        if (typeof val === 'string') {
            val = val.trim().replace(',', '.').replace(/[^0-9.]/g, '');
        }
        const n = Number(val);
        return Number.isFinite(n) ? Number(n.toFixed(2)) : 0;
    };

    const displayText = (val) => {
        return (val != null && String(val).trim() !== '') ? val : '-';
    };

    const escapeHtml = (str) => {
        return String(str ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    };

    const extractErrorMessage = (errorOrData, fallback = 'No se pudo guardar.') => {
        const status = errorOrData?.response?.status;
        const data = errorOrData?.response?.data ?? errorOrData;

        if (status === 403 || status === 401) {
            return 'No tienes permiso para realizar esta acción (Acceso denegado).';
        }

        if (!data) return fallback;

        if (typeof data === 'string') {
            const clean = data.trim();
            if (!clean) return fallback;
            if (clean.toLowerCase().includes('acceso denegado') || clean.toLowerCase().includes('<html') || clean.toLowerCase().includes('<!doctype')) {
                return 'No tienes permiso o sesión válida para realizar esta acción.';
            }
            return clean;
        }

        if (data.errorMessage) return data.errorMessage;
        if (data.message) return data.message;
        if (data.detail) return data.detail;
        if (data.title && !data.errors) return data.title;
        if (data.errors && typeof data.errors === 'object') {
            const flat = Object.values(data.errors).flat().filter(Boolean);
            if (flat.length) return flat.join(' | ');
        }

        return fallback;
    };

    const clearDetalleValidationMarks = () => {
        $('#ProductoId, #Cantidad, #PrecioUnitario').removeClass('is-invalid');
    };

    const renderDetallesCompra = () => {
        const tbody = document.getElementById('tbodyDetalleCompra');
        const lblTotal = document.getElementById('lblTotalCompra');
        if (!tbody) return;

        tbody.innerHTML = '';
        let total = 0;

        detallesCompra.forEach((item, index) => {
            const subtotal = toSafeDecimal(item.cantidad * item.precioUnitario);
            total = toSafeDecimal(total + subtotal);

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${escapeHtml(item.productoNombre)}</td>
                <td>${item.cantidad}</td>
                <td>${Number(item.precioUnitario).toFixed(2)}</td>
                <td>${subtotal.toFixed(2)}</td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-danger btn-quitar-detalle" data-index="${index}" title="Quitar">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });

        if (lblTotal) {
            lblTotal.textContent = total.toFixed(2);
        }
    };

    const formEl = document.getElementById('compraForm');
    const compraValidator = formEl ? new JustValidate('#compraForm', {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    }) : null;

    if (compraValidator && document.getElementById('ProveedorId')) {
        compraValidator
            .addField('#ProveedorId', [
                { rule: 'required', errorMessage: 'Seleccione un proveedor.' }
            ])
            .addField('#EmpleadoId', [
                { rule: 'required', errorMessage: 'Seleccione un empleado.' }
            ])
            .addField('#AlmacenId', [
                { rule: 'required', errorMessage: 'Seleccione un almacén.' }
            ]);
    }

    const table = $('#compraTable').DataTable({
        ajax: {
            url: '/Compra/GetData',
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
                data: 'fechaCompra',
                render: function (value) {
                    if (!value) return '-';
                    const fecha = new Date(value);
                    return isNaN(fecha.getTime()) ? value : fecha.toLocaleDateString('es-NI');
                }
            },
            {
                data: 'proveedorNombre',
                render: (value) => displayText(value)
            },
            {
                data: 'empleadoNombre',
                render: (value) => displayText(value)
            },
            {
                data: 'almacenNombre',
                render: (value) => displayText(value)
            },
            {
                data: 'sucursalNombre',
                render: (value) => displayText(value)
            },
            {
                data: 'total',
                render: function (value) {
                    return value != null ? Number(value).toFixed(2) : '0.00';
                }
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-primary me-1 btn-edit" data-id="${data.id}" title="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button type="button" class="btn btn-sm btn-outline-danger btn-delete" data-id="${data.id}" data-nombre="Compra #${data.id}" title="Eliminar">
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
        order: [[0, 'desc']]
    });

    $('#ProductoId').on('change', function () {
        $('#ProductoId').removeClass('is-invalid');
        const selectedOption = this.options[this.selectedIndex];
        const precioAttr = selectedOption?.getAttribute('data-precio');
        if (precioAttr) {
            const parsedPrecio = toSafeDecimal(precioAttr);
            if (parsedPrecio > 0) {
                $('#PrecioUnitario').val(parsedPrecio.toFixed(2)).removeClass('is-invalid');
            }
        }
    });

    $('#Cantidad').on('input', function () {
        const clean = String(this.value ?? '').replace(/\D/g, '').slice(0, 9);
        if (this.value !== clean) this.value = clean;
        $(this).removeClass('is-invalid');
    });

    $('#PrecioUnitario').on('input', function () {
        const clean = String(this.value ?? '').replace(/[^0-9.]/g, '').slice(0, 12);
        if (this.value !== clean) this.value = clean;
        $(this).removeClass('is-invalid');
    });

    $('#btnAgregarDetalle').on('click', function () {
        clearDetalleValidationMarks();

        const productoSelect = document.getElementById('ProductoId');
        const productoId = toSafeInt(productoSelect?.value);
        const productoNombre = productoId > 0
            ? (productoSelect?.options[productoSelect.selectedIndex]?.text || '').trim()
            : '';
        const cantidad = toSafeInt($('#Cantidad').val());
        const precioUnitario = toSafeDecimal($('#PrecioUnitario').val());

        let invalid = false;
        if (!productoId || productoId <= 0) {
            $('#ProductoId').addClass('is-invalid');
            invalid = true;
        }
        if (cantidad <= 0) {
            $('#Cantidad').addClass('is-invalid');
            invalid = true;
        }
        if (precioUnitario <= 0) {
            $('#PrecioUnitario').addClass('is-invalid');
            invalid = true;
        }

        if (invalid) {
            Swal.fire({
                icon: 'warning',
                title: 'Datos inválidos',
                text: 'Selecciona un producto, ingresa una cantidad entera mayor a 0 y un precio mayor a 0.'
            });
            return;
        }

        const existente = detallesCompra.find(d => d.productoId === productoId);
        if (existente) {
            existente.cantidad += cantidad;
            existente.precioUnitario = precioUnitario;
        } else {
            detallesCompra.push({
                productoId,
                productoNombre,
                cantidad,
                precioUnitario
            });
        }

        renderDetallesCompra();
        $('#ProductoId').val('');
        $('#Cantidad').val('');
        $('#PrecioUnitario').val('');
        clearDetalleValidationMarks();
    });

    $('#tbodyDetalleCompra').on('click', '.btn-quitar-detalle', function () {
        const index = toSafeInt($(this).data('index'));
        if (index >= 0 && index < detallesCompra.length) {
            detallesCompra.splice(index, 1);
            renderDetallesCompra();
        }
    });

    function clearForm() {
        $('#Id').val('');
        $('#ProveedorId').val('');
        $('#EmpleadoId').val('');
        $('#AlmacenId').val('');
        $('#ProductoId').val('');
        $('#Cantidad').val('');
        $('#PrecioUnitario').val('');
        clearDetalleValidationMarks();
        detallesCompra = [];
        renderDetallesCompra();
        compraValidator?.refresh();
    }

    $('#btnNuevaCompra').on('click', function () {
        isEdit = false;
        clearForm();
        $('#compraModalTitle').text('Nueva Compra');
        modal?.show();
    });

    $('#compraTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.get(`/Compra/GetById?id=${id}`);
            const data = response.data;
            clearForm();
            isEdit = true;
            $('#Id').val(data.id);
            $('#ProveedorId').val(data.proveedorId || '');
            $('#EmpleadoId').val(data.empleadoId || '');
            $('#AlmacenId').val(data.almacenId || '');

            const rawDetalles = data.detalleCompraDtos ?? data.detalles ?? data.items ?? [];
            detallesCompra = rawDetalles
                .map(d => ({
                    productoId: toSafeInt(d.productoId),
                    productoNombre: d.productoNombre ?? d.productoName ?? `Producto #${d.productoId}`,
                    cantidad: toSafeInt(d.cantidad ?? 0),
                    precioUnitario: toSafeDecimal(d.precioUnitario ?? d.precio ?? 0)
                }))
                .filter(d => d.productoId > 0 && d.cantidad > 0 && d.precioUnitario > 0);

            renderDetallesCompra();

            $('#compraModalTitle').text('Editar Compra');
            compraValidator?.refresh();
            modal?.show();
        } catch (error) {
            Swal.fire('Error', extractErrorMessage(error, 'No se pudo cargar la compra.'), 'error');
        }
    });

    $('#compraTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre');

        const confirm = await Swal.fire({
            title: '¿Eliminar compra?',
            html: `¿Seguro que deseas eliminar <strong>${escapeHtml(nombre)}</strong>?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d'
        });

        if (!confirm.isConfirmed) return;

        try {
            const response = await axios.post(`/Compra/Delete?id=${id}`);
            if (response.data.success) {
                table.ajax.reload(null, false);
                Swal.fire('Eliminado', response.data.message, 'success');
            } else {
                Swal.fire('Error', extractErrorMessage(response.data, 'No se pudo eliminar.'), 'error');
            }
        } catch (error) {
            Swal.fire('Error', extractErrorMessage(error, 'No se pudo eliminar.'), 'error');
        }
    });

    compraValidator?.onSuccess(async (event) => {
        event.preventDefault();

        if (detallesCompra.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Detalle vacío',
                text: 'Agrega al menos un producto con cantidad y precio mayores a 0.'
            });
            return;
        }

        const totalCalculado = toSafeDecimal(
            detallesCompra.reduce((acc, d) => acc + toSafeDecimal(d.cantidad * d.precioUnitario), 0)
        );

        const payload = {
            id: $('#Id').val() ? toSafeInt($('#Id').val()) : 0,
            proveedorId: toSafeInt($('#ProveedorId').val()),
            empleadoId: toSafeInt($('#EmpleadoId').val()),
            almacenId: toSafeInt($('#AlmacenId').val()),
            total: totalCalculado,
            detalleCompraDtos: detallesCompra.map(d => {
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
            const response = isEdit
                ? await axios.post(`/Compra/Update?id=${payload.id}`, payload)
                : await axios.post('/Compra/Create', payload);

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
                Swal.fire('Error', extractErrorMessage(response.data, 'No se pudo guardar.'), 'error');
            }
        } catch (error) {
            Swal.fire('Error', extractErrorMessage(error, 'No se pudo guardar.'), 'error');
        }
    });
});