import { all, fork } from "redux-saga/effects";
import { permissionsRootSaga } from "./permissions/permissionsSaga";

export function* rootSaga() {
  yield all([fork(permissionsRootSaga)]);
}
