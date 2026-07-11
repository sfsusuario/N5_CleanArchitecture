import { Alert, Button, Spinner, Table } from "react-bootstrap";
import type { Permission } from "../types/permission";

interface PermissionsListProps {
  items: Permission[];
  loading: boolean;
  error: string | null;
  onEdit: (permission: Permission) => void;
}

export function PermissionsList({ items, loading, error, onEdit }: PermissionsListProps) {
  if (loading) {
    return (
      <div className="d-flex align-items-center gap-2">
        <Spinner animation="border" size="sm" />
        <span>Cargando permisos…</span>
      </div>
    );
  }

  if (error) {
    return <Alert variant="danger">{error}</Alert>;
  }

  if (items.length === 0) {
    return <Alert variant="secondary">No hay permisos registrados todavía.</Alert>;
  }

  return (
    <Table striped bordered hover responsive>
      <thead>
        <tr>
          <th>Id</th>
          <th>Empleado</th>
          <th>Tipo de permiso</th>
          <th>Fecha</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        {items.map((permission) => (
          <tr key={permission.id}>
            <td>{permission.id}</td>
            <td>
              {permission.employeeForename} {permission.employeeSurname}
            </td>
            <td>{permission.permissionTypeRef?.description ?? permission.permissionType}</td>
            <td>{new Date(permission.permissionDate).toLocaleDateString()}</td>
            <td>
              <Button size="sm" variant="outline-primary" onClick={() => onEdit(permission)}>
                Editar
              </Button>
            </td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
}
