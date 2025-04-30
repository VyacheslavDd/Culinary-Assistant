import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { User } from '../types';
import {
    AuthUserData,
    loginUserApi,
    registerUserApi,
    RegisterUserData,
} from './api';

type TUserState = {
    user: User | null;
    error: string | null | undefined;
    isAuthenticated: boolean;
    isAuthChecked: boolean;
    isLoading: boolean;
};

export const initialState: TUserState = {
    user: null,
    error: null,
    isAuthenticated: false,
    isAuthChecked: false,
    isLoading: false,
};

// Регистрация пользователя
export const registerUser = createAsyncThunk(
    'user/register',
    async (data: RegisterUserData, { rejectWithValue }) => {
        try {
            return await registerUserApi(data);
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch User');
        }
    }
);

// Вход пользователя
export const loginUser = createAsyncThunk(
    'user/login',
    async (data: AuthUserData, { rejectWithValue }) => {
        try {
            return await loginUserApi(data);
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch User');
        }
    }
);

export const userSlice = createSlice({
    name: 'user',
    initialState,
    selectors: {
        selectUser: (state) => state.user,
        selectIsAuthenticated: (state) => state.isAuthenticated,
        selectIsAuthChecked: (state) => state.isAuthChecked,
        selectUserError: (state) => state.error,
        selectUserLoading: (state) => state.isLoading,
    },
    reducers: {},
    extraReducers: (builder) => {
        builder
            // Регистрация
            .addCase(registerUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(registerUser.fulfilled, (state, action) => {
                state.isLoading = false;

                state.isAuthenticated = true;
                state.user = action.payload;
            })
            .addCase(registerUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // Вход
            .addCase(loginUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(loginUser.fulfilled, (state, action) => {
                state.isLoading = false;

                state.isAuthenticated = true;
                state.user = action.payload;
            })
            .addCase(loginUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            });
    },
});

export const {
    selectUser,
    selectIsAuthenticated,
    selectIsAuthChecked,
    selectUserError,
    selectUserLoading,
} = userSlice.selectors;

export default userSlice.reducer;
