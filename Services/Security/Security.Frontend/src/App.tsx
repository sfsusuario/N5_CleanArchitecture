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
import type { Permission } from "./types/permission";

export default function App() {
  const dispatch = useAppDispatch();
  const { items, loading, submitting, error } = useAppSelector((state) => state.permissions);
  const [editingPermission, setEditingPermission] = useState<Permission | null>(null);

  useEffect(() => {
    dispatch(fetchPermissionsRequested());
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
        onSubmit={handleSubmit}
        onCancelEdit={() => setEditingPermission(null)}
      />
      <PermissionsList items={items} loading={loading} error={error} onEdit={setEditingPermission} />
    </Layout>
  );
}
