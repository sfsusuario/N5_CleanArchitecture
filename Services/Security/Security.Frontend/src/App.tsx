import { useEffect, useState } from "react";
import { Layout } from "./components/Layout";
import { PermissionForm, type PermissionFormValues } from "./components/PermissionForm";
import { PermissionsList } from "./components/PermissionsList";
import { useAppDispatch, useAppSelector } from "./store/hooks";
import {
  fetchPermissionsRequested,
  modifyPermissionRequested,
  requestPermissionRequested,
} from "./store/permissions/permissionsSlice";
import {
  createPermissionTypeRequested,
  fetchPermissionTypesRequested,
} from "./store/permissionTypes/permissionTypesSlice";
import type { Permission } from "./types/permission";

export default function App() {
  const dispatch = useAppDispatch();
  const { items, loading, submitting, error } = useAppSelector((state) => state.permissions);
  const {
    items: permissionTypes,
    loading: permissionTypesLoading,
    submitting: permissionTypesSubmitting,
  } = useAppSelector((state) => state.permissionTypes);
  const [editingPermission, setEditingPermission] = useState<Permission | null>(null);

  useEffect(() => {
    dispatch(fetchPermissionsRequested());
    dispatch(fetchPermissionTypesRequested());
  }, [dispatch]);

  const handleSubmit = (values: PermissionFormValues, editingId: number | null) => {
    if (editingId) {
      dispatch(modifyPermissionRequested({ id: editingId, ...values }));
      setEditingPermission(null);
    } else {
      dispatch(requestPermissionRequested(values));
    }
  };

  return (
    <Layout>
      <PermissionForm
        editingPermission={editingPermission}
        submitting={submitting}
        permissionTypes={permissionTypes}
        permissionTypesLoading={permissionTypesLoading}
        permissionTypesSubmitting={permissionTypesSubmitting}
        onSubmit={handleSubmit}
        onCancelEdit={() => setEditingPermission(null)}
        onCreatePermissionType={(description) =>
          dispatch(createPermissionTypeRequested({ description }))
        }
      />
      <PermissionsList items={items} loading={loading} error={error} onEdit={setEditingPermission} />
    </Layout>
  );
}
