import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { Ingredient } from '../types';


// export const fetchIngredients = createAsyncThunk(
//   'ingredients/fetchIngredients',
//   getIngredientsApi
// );

type TIngredientsState = {
  items: Ingredient[];
  isLoading: boolean;
};

export const initialState: TIngredientsState = {
  items: [],
  isLoading: false
};

export const ingredientsSlice = createSlice({
  name: 'ingredients',
  initialState,
  selectors: {
    selectIngredients: (state) => state.items,
    selectIngredientsLoading: (state) => state.isLoading
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
    //   .addCase(fetchIngredients.pending, (state) => {
    //     state.isLoading = true;
    //   })
    //   .addCase(fetchIngredients.fulfilled, (state, action) => {
    //     state.isLoading = false;
    //     state.items = action.payload;
    //   })
    //   .addCase(fetchIngredients.rejected, (state) => {
    //     state.isLoading = false;
    //   });
  }
});

export const { selectIngredients, selectIngredientsLoading } =
  ingredientsSlice.selectors;

// export const selectIngredientById = (id: string) => (state: RootState) =>
//   state.ingredients.items.find((item) => item._id === id);

export default ingredientsSlice.reducer;
