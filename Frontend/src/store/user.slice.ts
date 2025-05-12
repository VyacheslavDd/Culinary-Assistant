import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { ShortRecipe, User } from '../types';
import {
    AuthUserData,
    checkUserApi,
    getCollectionsByUserApi,
    getRecipesByUserApi,
    loginUserApi,
    logoutUserApi,
    registerUserApi,
    RegisterUserData,
} from './api';
import { ShortCollection } from 'types/short-collection.type';

type TUserState = {
    user: User | null;
    collections: ShortCollection[];
    recipes: ShortRecipe[];
    error: string | null | undefined;
    isAuthenticated: boolean;
    isAuthChecked: boolean;
    isLoading: boolean;
};

export const initialState: TUserState = {
    user: null,
    collections: [],
    recipes: [],
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
    async (data: AuthUserData, { rejectWithValue, dispatch }) => {
        try {
            const user = await loginUserApi(data);

            await Promise.all([
                dispatch(fetchUsersRecipes(user.id)),
                dispatch(fetchUsersCollections(user.id)),
            ]);

            return user;
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch User');
        }
    }
);

// Проверка входа пользователя
export const checkUser = createAsyncThunk<User, void, { rejectValue: string }>(
    'user/check',
    async (_, { rejectWithValue, dispatch }) => {
        try {
            const user = await checkUserApi();

            await Promise.all([
                dispatch(fetchUsersRecipes(user.id)),
                dispatch(fetchUsersCollections(user.id)),
            ]);

            return user;
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to check user');
        }
    }
);

// Выход пользователя
export const logoutUser = createAsyncThunk<void, void, { rejectValue: string }>(
    'user/logout',
    async (_, { rejectWithValue }) => {
        try {
            await logoutUserApi();
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to logout');
        }
    }
);

// Получение коллекций пользователя
export const fetchUsersCollections = createAsyncThunk(
    'user/fetchCollections',
    async (userId: string, { rejectWithValue }) => {
        try {
            return await getCollectionsByUserApi({ userId });
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch users collections');
        }
    }
);

// Получение рецептов пользователя
export const fetchUsersRecipes = createAsyncThunk(
    'user/fetchRecipes',
    async (userId: string, { rejectWithValue }) => {
        try {
            return await getRecipesByUserApi({ userId });
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch users recipes');
        }
    }
);
export const userSlice = createSlice({
    name: 'user',
    initialState,
    selectors: {
        selectUser: (state) => state.user,
        selectUserRecipes: (state) => state.recipes,
        selectUserCollections: (state) => state.collections,
        selectIsAuthenticated: (state) => state.isAuthenticated,
        selectIsAuthChecked: (state) => state.isAuthChecked,
        selectUserError: (state) => state.error,
        selectUserLoading: (state) => state.isLoading,
        selectRecipeById: (state, id) =>
            state.recipes.find((item) => item.id === id),
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
            })

            // Проверка входа
            .addCase(checkUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(checkUser.fulfilled, (state, action) => {
                state.isLoading = false;
                state.isAuthChecked = true;
                state.isAuthenticated = true;
                state.user = action.payload;
            })
            .addCase(checkUser.rejected, (state, action) => {
                state.isLoading = false;
                state.isAuthChecked = true;
                state.isAuthenticated = false;
                state.user = null;
            })

            // Выход
            .addCase(logoutUser.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(logoutUser.fulfilled, (state) => {
                state.isLoading = false;
                state.isAuthenticated = false;
                state.collections = [];
                state.recipes = [];
                state.user = null;
            })
            .addCase(logoutUser.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // Получение коллекций пользователя
            .addCase(fetchUsersCollections.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchUsersCollections.fulfilled, (state, action) => {
                state.isLoading = false;
                state.collections = action.payload.data || [];
            })
            .addCase(fetchUsersCollections.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            })

            // Получение рецептов пользователя
            .addCase(fetchUsersRecipes.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchUsersRecipes.fulfilled, (state, action) => {
                state.isLoading = false;
                state.recipes = action.payload.data || [];
            })
            .addCase(fetchUsersRecipes.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
            });
    },
});

export const {
    selectUser,
    selectUserRecipes,
    selectUserCollections,
    selectIsAuthenticated,
    selectIsAuthChecked,
    selectUserError,
    selectUserLoading,
    selectRecipeById
} = userSlice.selectors;

export default userSlice.reducer;
