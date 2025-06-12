import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { getIngredientsApi } from './api';

export const fetchIngredients = createAsyncThunk(
    'ingredients/fetchIngredients',
    getIngredientsApi
);

type TIngredientsState = {
    items: string[];
    isLoading: boolean;
};

export const initialState: TIngredientsState = {
    items: [],
    isLoading: false,
};

export const ingredientsSlice = createSlice({
    name: 'ingredients',
    initialState,
    selectors: {
        selectIngredients: (state) => state.items,
        selectIngredientsLoading: (state) => state.isLoading,
    },
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchIngredients.pending, (state) => {
                state.isLoading = true;
            })
            .addCase(fetchIngredients.fulfilled, (state, action) => {
                state.isLoading = false;
                state.items = action.payload.sort();
            })
            .addCase(fetchIngredients.rejected, (state) => {
                state.isLoading = false;
            });
    },
});

export const { selectIngredients, selectIngredientsLoading } =
    ingredientsSlice.selectors;

export default ingredientsSlice.reducer;
