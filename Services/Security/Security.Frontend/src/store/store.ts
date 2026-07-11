import { configureStore } from "@reduxjs/toolkit";
import createSagaMiddleware from "redux-saga";
import permissionsReducer from "./permissions/permissionsSlice";
import { rootSaga } from "./rootSaga";

const sagaMiddleware = createSagaMiddleware();

export const store = configureStore({
  reducer: {
    permissions: permissionsReducer,
  },
  // Async flow is handled entirely by sagas here, so the default thunk middleware is dropped
  // in favor of sagaMiddleware — keeps a single, explicit place for side effects.
  middleware: (getDefaultMiddleware) => getDefaultMiddleware({ thunk: false }).concat(sagaMiddleware),
});

sagaMiddleware.run(rootSaga);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
