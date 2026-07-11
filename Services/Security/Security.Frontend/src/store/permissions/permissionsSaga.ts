import { call, put, takeEvery, all, fork } from "redux-saga/effects";
import type { PayloadAction } from "@reduxjs/toolkit";
import { permissionsApi } from "../../api/permissionsApi";
import type {
  ModifyPermissionPayload,
  Permission,
  RequestPermissionPayload,
} from "../../types/permission";
import {
  fetchPermissionsRequested,
  fetchPermissionsSucceeded,
  fetchPermissionsFailed,
  requestPermissionRequested,
  requestPermissionSucceeded,
  requestPermissionFailed,
  modifyPermissionRequested,
  modifyPermissionSucceeded,
  modifyPermissionFailed,
} from "./permissionsSlice";

function errorMessage(error: unknown): string {
  if (error && typeof error === "object" && "message" in error) {
    return String((error as { message: unknown }).message);
  }
  return "Unexpected error";
}

export function* fetchPermissionsSaga() {
  try {
    const items: Permission[] = yield call(permissionsApi.getPermissions);
    yield put(fetchPermissionsSucceeded(items));
  } catch (error) {
    yield put(fetchPermissionsFailed(errorMessage(error)));
  }
}

export function* requestPermissionSaga(action: PayloadAction<RequestPermissionPayload>) {
  try {
    yield call(permissionsApi.requestPermission, action.payload);
    yield put(requestPermissionSucceeded());
    // Refresh the list so the newly created permission shows up.
    yield put(fetchPermissionsRequested());
  } catch (error) {
    yield put(requestPermissionFailed(errorMessage(error)));
  }
}

export function* modifyPermissionSaga(action: PayloadAction<ModifyPermissionPayload>) {
  try {
    yield call(permissionsApi.modifyPermission, action.payload);
    yield put(modifyPermissionSucceeded());
    yield put(fetchPermissionsRequested());
  } catch (error) {
    yield put(modifyPermissionFailed(errorMessage(error)));
  }
}

function* watchFetchPermissions() {
  yield takeEvery(fetchPermissionsRequested.type, fetchPermissionsSaga);
}

function* watchRequestPermission() {
  yield takeEvery(requestPermissionRequested.type, requestPermissionSaga);
}

function* watchModifyPermission() {
  yield takeEvery(modifyPermissionRequested.type, modifyPermissionSaga);
}

export function* permissionsRootSaga() {
  yield all([fork(watchFetchPermissions), fork(watchRequestPermission), fork(watchModifyPermission)]);
}
