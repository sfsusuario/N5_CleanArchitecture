import { all, fork } from "redux-saga/effects";
import { permissionsRootSaga } from "./permissions/permissionsSaga";
import { permissionTypesRootSaga } from "./permissionTypes/permissionTypesSaga";

export function* rootSaga() {
  yield all([fork(permissionsRootSaga), fork(permissionTypesRootSaga)]);
}
