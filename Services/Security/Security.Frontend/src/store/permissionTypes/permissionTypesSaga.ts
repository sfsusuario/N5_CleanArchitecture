import { call, put, takeEvery, all, fork } from "redux-saga/effects";
import { permissionTypesApi } from "../../api/permissionTypesApi";
import type { PermissionType } from "../../types/permission";
import {
  fetchPermissionTypesRequested,
  fetchPermissionTypesSucceeded,
  fetchPermissionTypesFailed,
} from "./permissionTypesSlice";

function errorMessage(error: unknown): string {
  if (error && typeof error === "object" && "message" in error) {
    return String((error as { message: unknown }).message);
  }
  return "Unexpected error";
}

export function* fetchPermissionTypesSaga() {
  try {
    const items: PermissionType[] = yield call(permissionTypesApi.getPermissionTypes);
    yield put(fetchPermissionTypesSucceeded(items));
  } catch (error) {
    yield put(fetchPermissionTypesFailed(errorMessage(error)));
  }
}

function* watchFetchPermissionTypes() {
  yield takeEvery(fetchPermissionTypesRequested.type, fetchPermissionTypesSaga);
}

export function* permissionTypesRootSaga() {
  yield all([fork(watchFetchPermissionTypes)]);
}
