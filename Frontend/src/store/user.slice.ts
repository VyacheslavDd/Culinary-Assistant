import { createSlice } from '@reduxjs/toolkit';
import { User } from '../types';

type TUserState = {
    user: User | null;
    error: string | null | undefined;
    isAuthenticated: boolean;
    isAuthChecked: boolean;
};

export const initialState: TUserState = {
    user: null,
    error: null,
    isAuthenticated: false,
    isAuthChecked: false,
};

// export const fetchUser = createAsyncThunk('user/fetchUser', getUserApi);
// export const updateUser = createAsyncThunk('user/updateUser', updateUserApi);
// export const logout = createAsyncThunk('user/logout', logoutApi);
// export const forgotPassword = createAsyncThunk(
//   'user/forgotPassword',
//   forgotPasswordApi
// );

export const userSlice = createSlice({
    name: 'user',
    initialState,
    selectors: {
        selectUser: (state) => state.user,
        selectIsAuthenticated: (state) => state.isAuthenticated,
        selectIsAuthChecked: (state) => state.isAuthChecked,
        selectUserError: (state) => state.error,
    },
    reducers: {}
    // extraReducers: (builder) => {
    //     builder;
        //   //fetchUser
        //   .addCase(fetchUser.pending, (state) => {
        //     state.error = null;
        //   })
        //   .addCase(fetchUser.fulfilled, (state, action) => {
        //     state.isAuthChecked = true;
        //     state.user = action.payload.user;
        //     state.isAuthenticated = true;
        //   })
        //   .addCase(fetchUser.rejected, (state, action) => {
        //     state.isAuthChecked = true;
        //     state.error = action.error.message;
        //   })
    // },
});

export const {
    selectUser,
    selectIsAuthenticated,
    selectIsAuthChecked,
    selectUserError,
} = userSlice.selectors;

export default userSlice.reducer;
