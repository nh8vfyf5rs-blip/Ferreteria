document.addEventListener('DOMContentLoaded', () => {
    async function refreshSessionIfNeeded() {
        if (typeof refreshSession === 'function') {
            await refreshSession(false);
        }
    }

    function showSuccess(title, text) {
        return Swal.fire({
            icon: 'success',
            title,
            text,
            timer: 1500,
            showConfirmButton: false
        });
    }

    function showError(text) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: text || 'No se pudo completar la operación.'
        });
    }

    async function toggleRolePermission(roleId, permissionId, action) {
        const url = action === 'assign'
            ? `/Admin/AssignPermissionToRole?roleId=${roleId}&permissionId=${permissionId}`
            : `/Admin/RemovePermissionFromRole?roleId=${roleId}&permissionId=${permissionId}`;

        const response = await axios.post(url);
        return response.data;
    }

    document.querySelectorAll('.btn-toggle-permission').forEach(button => {
        button.addEventListener('click', async () => {
            try {
                const result = await toggleRolePermission(
                    button.dataset.roleId,
                    button.dataset.permissionId,
                    button.dataset.action
                );

                if (result.success) {
                    await refreshSessionIfNeeded();
                    await showSuccess('Listo', result.message);
                    location.reload();
                } else {
                    showError(result.message);
                }
            } catch (err) {
                showError(err.response?.data?.message);
            }
        });
    });

    const assignBtn = document.getElementById('btnAssignPermission');
    const permissionSelect = document.getElementById('permissionToAssign');
    if (assignBtn && permissionSelect) {
        assignBtn.addEventListener('click', async () => {
            try {
                const result = await toggleRolePermission(
                    assignBtn.dataset.roleId,
                    permissionSelect.value,
                    'assign'
                );

                if (result.success) {
                    await refreshSessionIfNeeded();
                    await showSuccess('Permiso asignado', result.message);
                    location.reload();
                } else {
                    showError(result.message);
                }
            } catch (err) {
                showError(err.response?.data?.message);
            }
        });
    }

    const updateRoleBtn = document.getElementById('btnUpdateRole');
    if (updateRoleBtn) {
        updateRoleBtn.addEventListener('click', async () => {
            const userId = updateRoleBtn.dataset.userId;
            const roleId = document.getElementById('userRoleSelect')?.value;

            try {
                const response = await axios.post(`/Admin/UpdateRole?userId=${userId}`, {
                    rolId: parseInt(roleId, 10)
                });

                if (response.data.success) {
                    await refreshSessionIfNeeded();
                    await showSuccess('Rol actualizado', response.data.message);
                    location.reload();
                } else {
                    showError(response.data.message);
                }
            } catch (err) {
                showError(err.response?.data?.message);
            }
        });
    }

    async function assignUserPermission(userId, permissionId, isGranted) {
        const response = await axios.post(`/Admin/AssignPermissionToUser?userId=${userId}`, {
            permisoId: parseInt(permissionId, 10),
            isGranted
        });
        return response.data;
    }

    const userPermissionsTable = document.getElementById('userPermissionsTable');
    if (userPermissionsTable) {
        userPermissionsTable.addEventListener('click', async (event) => {
            const grantBtn = event.target.closest('.btn-user-grant');
            const denyBtn = event.target.closest('.btn-user-deny');
            const removeBtn = event.target.closest('.btn-user-remove');
            const button = grantBtn || denyBtn || removeBtn;
            if (!button) return;

            const userId = button.dataset.userId;
            const permissionId = button.dataset.permissionId;

            try {
                let result;

                if (grantBtn) {
                    result = await assignUserPermission(userId, permissionId, true);
                } else if (denyBtn) {
                    result = await assignUserPermission(userId, permissionId, false);
                } else {
                    const response = await axios.post(
                        `/Admin/RemoveUserPermission?userId=${userId}&permissionId=${permissionId}`
                    );
                    result = response.data;
                }

                if (result.success) {
                    await refreshSessionIfNeeded();
                    await showSuccess('Listo', result.message);
                    location.reload();
                } else {
                    showError(result.message);
                }
            } catch (err) {
                showError(err.response?.data?.message);
            }
        });
    }
});