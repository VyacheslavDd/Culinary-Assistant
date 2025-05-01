import { configureStore, combineReducers } from '@reduxjs/toolkit';
import {
    TypedUseSelectorHook,
    useDispatch as dispatchHook,
    useSelector as selectorHook,
} from 'react-redux';
import userReducer from './user.slice';
import ingredientsReducer from './ingredients.slice';
import mainPageReducer from './main-page.slice';
import collectionsPageReducer from './collections-page.slice';

export const rootReducer = combineReducers({
    user: userReducer,
    ingredients: ingredientsReducer,
    mainPage: mainPageReducer,
    collectionsPage: collectionsPageReducer,
});

const store = configureStore({
    reducer: rootReducer,
    devTools: process.env.NODE_ENV !== 'production',
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useDispatch: () => AppDispatch = dispatchHook;
export const useSelector: TypedUseSelectorHook<RootState> = selectorHook;

export default store;
