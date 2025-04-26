import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { Filter, ShortRecipe } from '../types';
import { getRecipesApi, TRecipesData } from './api';
import axios from 'axios';
import { RootState } from './store';

type TMainPageState = {
    page: number;
    recipes: ShortRecipe[];
    filter: Filter;
    error: string | null | undefined;
    isLoading: boolean;
    totalPages: number;
    totalRecipes: number;
};

export const initialState: TMainPageState = {
    page: 1,
    recipes: [],
    filter: {
        SearchByTitle: '',
        SearchByIngredients: [],
        CookingTimeFrom: 0,
        CookingTimeTo: 0,
        CookingDifficulty: [],
        Category: [],
        Tags: [],
    },
    error: null,
    isLoading: false,
    totalPages: 0,
    totalRecipes: 0,
};

export const fetchRecipes = createAsyncThunk(
    'recipes/fetchAll',
    async (_, { getState, rejectWithValue }) => {
        try {
            const state = getState() as RootState;
            const { page, filter } = state.mainPage;

            const requestParams: TRecipesData = {
                Page: page,
                ...filter,
            };

            return await getRecipesApi(requestParams);
        } catch (error) {
            if (axios.isAxiosError(error)) {
                return rejectWithValue(
                    error.response?.data?.message || error.message
                );
            }
            return rejectWithValue('Unknown error occurred');
        }
    }
);

export const mainPageSlice = createSlice({
    name: 'mainPage',
    initialState,
    selectors: {
        selectPage: (state) => state.page,
        selectRecipes: (state) => state.recipes,
        selectFilter: (state) => state.filter,
        selectMainPageError: (state) => state.error,
        selectMainPageLoading: (state) => state.isLoading,
        selectTotalPages: (state) => state.totalPages,
        selectTotalRecipes: (state) => state.totalRecipes,
    },
    reducers: {
        setPage: (state, action) => {
            state.page = action.payload;
        },
        updateFilter: (state, action) => {
            state.filter = { ...state.filter, ...action.payload };
        },
        resetFilters: (state) => {
            state.filter = initialState.filter;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchRecipes.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchRecipes.fulfilled, (state, action) => {
                state.isLoading = false;
                state.recipes = action.payload.data || [];
                state.totalPages = action.payload.pagesCount ?? 0;
                state.totalRecipes = action.payload.entitiesCount ?? 0;
            })
            .addCase(fetchRecipes.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
                state.recipes = [];
                state.totalPages = 0;
                state.totalRecipes = 0;
            });
    },
});

export const {
    selectMainPageError,
    selectMainPageLoading,
    selectPage,
    selectRecipes,
    selectFilter,
    selectTotalPages,
    selectTotalRecipes,
} = mainPageSlice.selectors;

export const { setPage, updateFilter, resetFilters } = mainPageSlice.actions;

export default mainPageSlice.reducer;
