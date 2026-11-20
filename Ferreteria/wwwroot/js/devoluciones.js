document.addEventListener("DOMContentLoaded", () => {
    const modalEl = document.getElementById("devolucionesModal");
    const modal = new bootstrap.Modal(modalEl);
    const form = document.getElementById("devolucionesForm");
    const modalTitle = document.getElementById("devolucionesModalTitle");
    const btnNueva = document.getElementById("btnNuevaDevolucion");

    const table = $("#devolucionesTable").DataTable({
        ajax: {
            url: "/Devoluciones/GetData",
            type: "GET",
            dataSrc: (json) => json.data ?? json
        },
        columns: [
            { data: "id" },
            {
                data: "fecha",
                render: (data) => data ? new Date(data).toLocaleDateString("es-NI") : ""
            },
            { data: "ventaReferencia" },
            { data: "motivo" },
            {
                data: "total",
                render: (data) => Number(data ?? 0).toFixed(2)
            },
            {
                data: "id",
                orderable: false,
                searchable: false,
                render: (id) => `
                    <div class="btn-group btn-group-sm">
                        <button type="button" class="btn btn-warning btn-editar" data-id="${id}">Editar</button>
                        <button type="button" class="btn btn-danger btn-eliminar" data-id="${id}">Eliminar</button>
                    </div>
                `
            }
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json"
        }
    });

    const resetForm = () => {
        form.reset();
        document.getElementById("Id").value = "0";
        document.getElementById("Fecha").valueAsDate = new Date();
    };

    btnNueva.addEventListener("click", () => {
        resetForm();
        modalTitle.textContent = "Nueva Devolución";
        modal.show();
    });

    const validator = new JustValidate("#devolucionesForm", {
        errorFieldCssClass: "is-invalid",
        successFieldCssClass: "is-valid"
    });

    validator
        .addField("#Fecha", [{ rule: "required", errorMessage: "La fecha es obligatoria" }])
        .addField("#VentaId", [
            { rule: "required", errorMessage: "La venta es obligatoria" },
            { rule: "number", errorMessage: "Debe ser un número válido" }
        ])
        .addField("#Total", [
            { rule: "required", errorMessage: "El total es obligatorio" },
            { rule: "number", errorMessage: "Ingrese un monto válido" }
        ])
        .addField("#Motivo", [
            { rule: "required", errorMessage: "El motivo es obligatorio" },
            { rule: "minLength", value: 3, errorMessage: "Mínimo 3 caracteres" }
        ])
        .onSuccess(async (event) => {
            event.preventDefault();

            const id = Number(document.getElementById("Id").value || 0);
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

                modal.hide();
                table.ajax.reload(null, false);
                Swal.fire("Éxito", data?.message || "Registro guardado correctamente.", "success");
            } catch (error) {
                console.error(error);
                Swal.fire("Error", "Ocurrió un error al guardar la devolución.", "error");
            }
        });

    $("#devolucionesTable tbody").on("click", ".btn-editar", async function () {
        const id = $(this).data("id");
        try {
            const { data } = await axios.get(`/Devoluciones/GetById?id=${id}`);
            const item = data?.data ?? data;

            resetForm();
            modalTitle.textContent = "Editar Devolución";
            document.getElementById("Id").value = item.id;
            document.getElementById("Fecha").value = (item.fecha || "").split("T")[0];
            document.getElementById("VentaId").value = item.ventaId ?? "";
            document.getElementById("Total").value = item.total ?? 0;
            document.getElementById("Motivo").value = item.motivo ?? "";

            modal.show();
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
            table.ajax.reload(null, false);
            Swal.fire("Eliminado", data?.message || "Registro eliminado.", "success");
        } catch (error) {
            console.error(error);
            Swal.fire("Error", "Ocurrió un error al eliminar el registro.", "error");
        }
    });
});
