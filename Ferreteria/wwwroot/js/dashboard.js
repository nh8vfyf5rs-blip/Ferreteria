document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("dashboardSearchInput");
    const salesTableBody = document.querySelector("#recentSalesTable tbody");

    if (searchInput && salesTableBody) {
        searchInput.addEventListener("input", function () {
            const term = this.value.trim().toLowerCase();
            const rows = salesTableBody.querySelectorAll("tr");

            rows.forEach(function (row) {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(term) ? "" : "none";
            });
        });
    }
});