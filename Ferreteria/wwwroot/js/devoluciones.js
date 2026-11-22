document.addEventListener("DOMContentLoaded", () => {
    const config = window.devolucionesConfig || { isGerente: false };
    const isGerente = Boolean(config.isGerente);

    const modalEl = document.getElementById("devolucionesModal");
    const modal = (!isGerente && window.bootstrap && modalEl) ? new bootstrap.Modal(modalEl) : null;

    const detalleModalEl = document.getElementById("detalleDevolucionModal");
    const detalleModal = (window.bootstrap && detalleModalEl) ? new bootstrap.Modal(detalleModalEl) : null;

    const form = document.getElementById("devolucionesForm");
    const modalTitle = document.getElementById("devolucionesModalTitle");
    const btnNueva = document.getElementById("btnNuevaDevolucion");

    const resetForm = () => {
        if (form) form.reset();
        const idInput = document.getElementById("Id");
        if (idInput) idInput.value = "0";
    };

    const mostrarModalForm = (titulo) => {
        if (isGerente) return;
        if (modalTitle && titulo) modalTitle.textContent = titulo;
        if (modal) {
            modal.show();
        } else if (modalEl) {
            modalEl.classList.add("show");
            modalEl.style.display = "block";
            modalEl.removeAttribute("aria-hidden");
        }
    };

    const abrirModalNueva = () => {
        if (isGerente) return;
        resetForm();
        mostrarModalForm("Nueva Devolución");
    };

    const cerrarModal = () => {
        if (!modalEl) return;
        if (modal) {
            modal.hide();
        } else {
            modalEl.classList.remove("show");
            modalEl.style.display = "none";
            modalEl.setAttribute("aria-hidden", "true");
        }
    };

    const mostrarModalDetalle = (item) => {
        const fechaTexto = item?.fechaDevolucion
            ? new Date(item.fechaDevolucion).toLocaleDateString("es-NI")
            : "-";

        document.getElementById("verId").textContent = item?.id ?? "-";
        document.getElementById("verFecha").textContent = fechaTexto;
        document.getElementById("verVenta").textContent = item?.ventaId ? `Venta #${item.ventaId}` : "-";
        document.getElementById("verCliente").textContent = item?.clienteNombre || "-";
        document.getElementById("verTotal").textContent = Number(item?.total ?? 0).toFixed(2);
        document.getElementById("verMotivo").textContent = item?.motivo || "-";

        if (detalleModal) {
            detalleModal.show();
        } else if (detalleModalEl) {
            detalleModalEl.classList.add("show");
            detalleModalEl.style.display = "block";
            detalleModalEl.removeAttribute("aria-hidden");
        }
    };

    if (btnNueva && !isGerente) {
        btnNueva.addEventListener("click", abrirModalNueva);
    }

    let table = null;
    if (window.$ && $.fn && $.fn.DataTable) {
        table = $("#devolucionesTable").DataTable({
            ajax: {
                url: "/Devoluciones/GetData",
                type: "GET",
                dataSrc: (json) => {
                    if (Array.isArray(json)) return json;
                    if (Array.isArray(json?.data)) return json.data;
                    if (Array.isArray(json?.items)) return json.items;
                    return [];
                },
                error: (xhr, status, err) => {
                    console.error("Error cargando devoluciones:", status, err, xhr?.responseText);
                }
            },
            columns: [
                { data: "id" },
                {
                    data: "fechaDevolucion",
                    render: (data) => data ? new Date(data).toLocaleDateString("es-NI") : ""
                },
                {
                    data: null,
                    render: (row) => {
                        const venta = row.ventaId ? `Venta #${row.ventaId}` : "";
                        const cliente = row.clienteNombre ? ` · ${row.clienteNombre}` : "";
                        return `${venta}${cliente}`;
                    }
                },
                { data: "motivo" },
                {
                    data: "total",
                    render: (data) => Number(data ?? 0).toFixed(2)
                },
                {
                    data: "id",
                    orderable: false,
                    searchable: false,
                    render: (id) => {
                        if (isGerente) {
                            return `
                                <div class="btn-group btn-group-sm">
                                    <button type="button" class="btn btn-info text-white btn-ver" data-id="${id}">Ver</button>
                                </div>
                            `;
                        }

                        return `
                            <div class="btn-group btn-group-sm">
                                <button type="button" class="btn btn-warning btn-editar" data-id="${id}">Editar</button>
                                <button type="button" class="btn btn-danger btn-eliminar" data-id="${id}">Eliminar</button>
                            </div>
                        `;
                    }
                }
            ]
        });
    } else {
        console.warn("DataTables no está cargado en el layout.");
    }

    if (!isGerente && window.JustValidate && form) {
        const validator = new JustValidate("#devolucionesForm", {
            errorFieldCssClass: "is-invalid",
            successFieldCssClass: "is-valid"
        });

        validator
            .addField("#VentaId", [
                { rule: "required", errorMessage: "La venta es obligatoria" },
                { rule: "number", errorMessage: "Debe ser un número válido" }
            ])
            .addField("#Motivo", [
                { rule: "required", errorMessage: "El motivo es obligatorio" },
                { rule: "minLength", value: 3, errorMessage: "Mínimo 3 caracteres" }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();

                const id = Number(document.getElementById("Id")?.value || 0);
                const url = id > 0 ? `/Devoluciones/Update?id=${id}` : "/Devoluciones/Create";
                const formData = new FormData(form);
                const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');

                try {
                    const { data } = await axios.post(url, formData, {
                        headers: tokenInput ? { "RequestVerificationToken": tokenInput.value } : {}
                    });

                    if (data?.success === false) {
                        Swal.fire("Atención", data.message || "No se pudo guardar.", "warning");
                        return;
                    }

                    cerrarModal();
                    if (table) table.ajax.reload(null, false);
                    Swal.fire("Éxito", data?.message || "Registro guardado correctamente.", "success");
                } catch (error) {
                    console.error(error);
                    Swal.fire("Error", "Ocurrió un error al guardar la devolución.", "error");
                }
            });
    }

    if (window.$) {
        $("#devolucionesTable tbody").on("click", ".btn-ver", async function () {
            const id = $(this).data("id");
            try {
                const { data } = await axios.get(`/Devoluciones/GetById?id=${id}`);
                if (data?.success === false) {
                    Swal.fire("Atención", data.message || "No se encontró la devolución.", "warning");
                    return;
                }
                const item = data?.data ?? data;
                mostrarModalDetalle(item);
            } catch (error) {
                console.error(error);
                Swal.fire("Error", "No se pudieron cargar los datos de la devolución.", "error");
            }
        });

        if (!isGerente) {
            $("#devolucionesTable tbody").on("click", ".btn-editar", async function () {
                const id = $(this).data("id");
                try {
                    const { data } = await axios.get(`/Devoluciones/GetById?id=${id}`);
                    const item = data?.data ?? data;

                    resetForm();
                    document.getElementById("Id").value = item.id ?? "0";
                    document.getElementById("VentaId").value = item.ventaId ?? "";
                    document.getElementById("Motivo").value = item.motivo ?? "";

                    mostrarModalForm("Editar Devolución");
                } catch (error) {
                    console.error(error);
                    Swal.fire("Error", "No se pudieron cargar los datos de la devolución.", "error");
                }
            });

            $("#devolucionesTable tbody").on("click", ".btn-eliminar", async function () {
                const id = $(this).data("id");

                const confirm = await Swal.fire({
                    title: "¿Eliminar devolución?",
                    text: "Esta acción no se puede deshacer.",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: "Sí, eliminar",
                    cancelButtonText: "Cancelar"
                });

                if (!confirm.isConfirmed) return;

                try {
                    const { data } = await axios.post(`/Devoluciones/Delete?id=${id}`);
                    if (data?.success === false) {
                        Swal.fire("Atención", data.message || "No se pudo eliminar.", "warning");
                        return;
                    }
                    if (table) table.ajax.reload(null, false);
                    Swal.fire("Eliminado", data?.message || "Registro eliminado.", "success");
                } catch (error) {
                    console.error(error);
                    Swal.fire("Error", "Ocurrió un error al eliminar el registro.", "error");
                }
            });
        }
    }
});