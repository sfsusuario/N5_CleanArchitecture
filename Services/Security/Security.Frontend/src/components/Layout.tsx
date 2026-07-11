import type { PropsWithChildren } from "react";
import { Container, Navbar } from "react-bootstrap";

export function Layout({ children }: PropsWithChildren) {
  return (
    <>
      <Navbar bg="dark" variant="dark" className="mb-4">
        <Container>
          <Navbar.Brand href="/">Security — Gestión de Permisos</Navbar.Brand>
        </Container>
      </Navbar>
      <Container className="pb-5">{children}</Container>
    </>
  );
}
