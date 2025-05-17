import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { getCollectionsApi } from './api';
import { RootState } from './store';
import { ShortCollection } from 'types/short-collection.type';
import { FilterCollection } from 'types/filter-collections.type';

type TCollectionsPageState = {
    page: number;
    collections: ShortCollection[];
    popularCollections: ShortCollection[];
    filter: FilterCollection;
    error: string | null | undefined;
    isLoading: boolean;
    totalPages: number;
    totalRecipes: number;
};

export const initialState: TCollectionsPageState = {
    page: 1,
    collections: [],
    popularCollections: [],
    filter: {
        Title: '',
        Limit: 15,
    },
    error: null,
    isLoading: false,
    totalPages: 0,
    totalRecipes: 0,
};

// Получение коллекций
export const fetchCollections = createAsyncThunk(
    'collections/fetchAll',
    async (_, { getState, rejectWithValue }) => {
        try {
            const state = getState() as RootState;
            const { page, filter } = state.collectionsPage;
            return await getCollectionsApi({ Page: page, ...filter });
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch collections');
        }
    }
);

// Получение популярных коллекций
export const fetchPopularCollections = createAsyncThunk(
    'collections/fetchAllPopular',
    async (_, { getState, rejectWithValue }) => {
        try {
            const state = getState() as RootState;
            const { page } = state.collectionsPage;
            return await getCollectionsApi({ Page: page, Limit: 10 });
        } catch (error) {
            if (error instanceof Error) {
                return rejectWithValue(error.message);
            }
            return rejectWithValue('Failed to fetch popular collections');
        }
    }
);

export const collectionsPageSlice = createSlice({
    name: 'collectionsPage',
    initialState,
    selectors: {
        selectPage: (state) => state.page,
        selectCollections: (state) => state.collections,
        selectPopularCollections: (state) => state.popularCollections,
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
            // Получение коллекций
            .addCase(fetchCollections.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchCollections.fulfilled, (state, action) => {
                state.isLoading = false;
                state.collections = action.payload.data || [];
                state.totalPages = action.payload.pagesCount ?? 0;
                state.totalRecipes = action.payload.entitiesCount ?? 0;
            })
            .addCase(fetchCollections.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
                state.collections = [];
                state.totalPages = 0;
                state.totalRecipes = 0;
            })

            // Получение популярных коллекций
            .addCase(fetchPopularCollections.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(fetchPopularCollections.fulfilled, (state, action) => {
                state.isLoading = false;
                state.popularCollections = action.payload.data || [];
            })
            .addCase(fetchPopularCollections.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload as string;
                state.popularCollections = [];
            });
    },
});

export const {
    selectMainPageError,
    selectMainPageLoading,
    selectPage,
    selectCollections,
    selectPopularCollections,
    selectFilter,
    selectTotalPages,
    selectTotalRecipes,
} = collectionsPageSlice.selectors;

export const { setPage, updateFilter, resetFilters } =
    collectionsPageSlice.actions;

export default collectionsPageSlice.reducer;
