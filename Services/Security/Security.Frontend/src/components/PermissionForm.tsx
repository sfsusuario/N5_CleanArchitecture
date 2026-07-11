import { useEffect, useState, type FormEvent } from "react";
import { Button, Card, Col, Form, Row } from "react-bootstrap";
import type { Permission, PermissionType } from "../types/permission";

export interface PermissionFormValues {
  employeeForename: string;
  employeeSurname: string;
  permissionType: number;
  permissionDate: string;
}

interface PermissionFormProps {
  editingPermission: Permission | null;
  submitting: boolean;
  permissionTypes: PermissionType[];
  permissionTypesLoading: boolean;
  onSubmit: (values: PermissionFormValues, editingId: number | null) => void;
  onCancelEdit: () => void;
}

const emptyForm: PermissionFormValues = {
  employeeForename: "",
  employeeSurname: "",
  permissionType: 0,
  permissionDate: new Date().toISOString().slice(0, 10),
};

export function PermissionForm({
  editingPermission,
  submitting,
  permissionTypes,
  permissionTypesLoading,
  onSubmit,
  onCancelEdit,
}: PermissionFormProps) {
  const [values, setValues] = useState<PermissionFormValues>(emptyForm);

  useEffect(() => {
    if (editingPermission) {
      setValues({
        employeeForename: editingPermission.employeeForename ?? "",
        employeeSurname: editingPermission.employeeSurname ?? "",
        permissionType: editingPermission.permissionType,
        permissionDate: editingPermission.permissionDate.slice(0, 10),
      });
    } else {
      setValues(emptyForm);
    }
  }, [editingPermission]);

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault();
    onSubmit(values, editingPermission?.id ?? null);
    if (!editingPermission) {
      setValues(emptyForm);
    }
  };

  return (
    <Card className="mb-4">
      <Card.Body>
        <Card.Title>{editingPermission ? `Editar permiso #${editingPermission.id}` : "Nueva solicitud de permiso"}</Card.Title>
        <Form onSubmit={handleSubmit}>
          <Row className="g-3">
            <Col md={6}>
              <Form.Group controlId="employeeForename">
                <Form.Label>Nombre</Form.Label>
                <Form.Control
                  required
                  value={values.employeeForename}
                  onChange={(e) => setValues({ ...values, employeeForename: e.target.value })}
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group controlId="employeeSurname">
                <Form.Label>Apellido</Form.Label>
                <Form.Control
                  required
                  value={values.employeeSurname}
                  onChange={(e) => setValues({ ...values, employeeSurname: e.target.value })}
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group controlId="permissionType">
                <Form.Label>Tipo de permiso</Form.Label>
                <Form.Select
                  required
                  disabled={permissionTypesLoading}
                  value={values.permissionType || ""}
                  onChange={(e) => setValues({ ...values, permissionType: Number(e.target.value) })}
                >
                  <option value="" disabled hidden>
                    {permissionTypesLoading ? "Cargando…" : "Seleccionar…"}
                  </option>
                  {permissionTypes.map((type) => (
                    <option key={type.id} value={type.id}>
                      {type.description ?? `Tipo #${type.id}`}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group controlId="permissionDate">
                <Form.Label>Fecha</Form.Label>
                <Form.Control
                  type="date"
                  required
                  value={values.permissionDate}
                  onChange={(e) => setValues({ ...values, permissionDate: e.target.value })}
                />
              </Form.Group>
            </Col>
          </Row>

          <div className="mt-3 d-flex gap-2">
            <Button type="submit" variant="primary" disabled={submitting}>
              {submitting ? "Guardando…" : editingPermission ? "Guardar cambios" : "Solicitar permiso"}
            </Button>
            {editingPermission && (
              <Button type="button" variant="outline-secondary" onClick={onCancelEdit} disabled={submitting}>
                Cancelar
              </Button>
            )}
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}
