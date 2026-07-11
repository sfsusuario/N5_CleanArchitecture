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
  permissionTypesSubmitting: boolean;
  onSubmit: (values: PermissionFormValues, editingId: number | null) => void;
  onCancelEdit: () => void;
  onCreatePermissionType: (description: string) => void;
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
  permissionTypesSubmitting,
  onSubmit,
  onCancelEdit,
  onCreatePermissionType,
}: PermissionFormProps) {
  const [values, setValues] = useState<PermissionFormValues>(emptyForm);
  const [showNewType, setShowNewType] = useState(false);
  const [newTypeDescription, setNewTypeDescription] = useState("");

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

  const handleCreateType = (event: FormEvent) => {
    event.preventDefault();
    if (!newTypeDescription.trim()) {
      return;
    }
    onCreatePermissionType(newTypeDescription.trim());
    setNewTypeDescription("");
    setShowNewType(false);
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
                <div className="d-flex justify-content-between align-items-center">
                  <Form.Label className="mb-0">Tipo de permiso</Form.Label>
                  <Button
                    type="button"
                    variant="link"
                    size="sm"
                    className="p-0"
                    onClick={() => setShowNewType((current) => !current)}
                  >
                    {showNewType ? "Cancelar" : "+ Nuevo tipo"}
                  </Button>
                </div>
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
                {showNewType && (
                  <div className="d-flex gap-2 mt-2">
                    <Form.Control
                      size="sm"
                      placeholder="Nombre del nuevo tipo (ej. Vacaciones)"
                      value={newTypeDescription}
                      onChange={(e) => setNewTypeDescription(e.target.value)}
                      disabled={permissionTypesSubmitting}
                    />
                    <Button
                      type="button"
                      size="sm"
                      variant="outline-primary"
                      onClick={handleCreateType}
                      disabled={permissionTypesSubmitting || !newTypeDescription.trim()}
                    >
                      {permissionTypesSubmitting ? "Agregando…" : "Agregar"}
                    </Button>
                  </div>
                )}
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
