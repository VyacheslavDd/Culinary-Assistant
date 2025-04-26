import axios from 'axios';
import qs from 'qs';
import {
    Category,
    CookingDifficulty,
    Ingredient,
    ShortRecipe,
    Tag,
} from 'types';

export type TRecipesData = {
    Page: number;
    Limit?: number;
    Tags?: Tag[];
    SearchByTitle?: string;
    SearchByIngredients?: Ingredient[];
    CookingTimeFrom?: number;
    CookingTimeTo?: number;
    Category?: Category[];
    CookingDifficulty?: CookingDifficulty[];
};

export type TRecipesResponse = {
    data: ShortRecipe[];
    entitiesCount: number;
    pagesCount: number;
};

const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5000/';

// Получение списка рецептов
export const getRecipesApi = async (
    data: TRecipesData
): Promise<TRecipesResponse> => {
    const params = Object.fromEntries(
        Object.entries(data).filter(
            ([_, value]) =>
                value !== undefined &&
                value !== null &&
                value !== '' &&
                (!Array.isArray(value) || value.length > 0)
        )
    );

    const response = await axios.get<TRecipesResponse>(
        `${apiUrl}api/receipts`,
        {
            params: params,
            paramsSerializer: (params) =>
                qs.stringify(params, { arrayFormat: 'repeat' }),
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
        }
    );

    return response.data;
};

export type TRecipeResponse = {
    data: ShortRecipe;
    entitiesCount?: number;
    pagesCount?: number;
};

// Получение отдельного рецепта
export const getRecipeByIdApi = async (
    id: string
): Promise<TRecipeResponse> => {
    try {
        const response = await axios.get<TRecipeResponse>(
            `${apiUrl}api/receipts/${id}`,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );
        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Failed to fetch recipe'
            );
        }
        throw new Error('Unknown error occurred');
    }
};

export type RegisterUserData = {
    login: string;
    emailOrPhone: string;
    password: string;
};

export type AuthResponse = {
    user: {
        userId: string;
        username: string;
        email: string;
    };
    token: string;
    expiresIn: number;
};

//Регистрация пользователя
export const registerUserApi = async (
    data: RegisterUserData
): Promise<AuthResponse> => {
    try {
        const response = await axios.post<AuthResponse>(
            `${apiUrl}api/auth/register`,
            data,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Registration failed'
            );
        }
        throw new Error('Unknown error occurred');
    }
};

export type AuthUserData = {
    login: string;
    password: string;
    adminEntrance: true;
    rememberMe: true;
};

//Аутентификация пользователя
export const loginUserApi = async (
    data: AuthUserData
): Promise<AuthResponse> => {
    try {
        const response = await axios.post<AuthResponse>(
            `${apiUrl}api/auth/login`,
            data,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(error.response?.data?.message || 'Login failed');
        }
        throw new Error('Unknown error occurred');
    }
};
