import axios from 'axios';
import qs from 'qs';
import {
    Category,
    CookingDifficulty,
    CookingStep,
    Ingredient,
    Measure,
    Recipe,
    ShortRecipe,
    ShortUser,
    Tag,
    User,
} from 'types';
import { Collection } from 'types/collections.type';
import { ShortCollection } from 'types/short-collections.type';

export type TRecipesData = {
    Page: number;
    Limit?: number;
    Tags?: Tag[];
    SearchByTitle?: string;
    // SearchByIngredients?: Ingredient[];
    SearchByIngredients?: string;
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
    try {
        const params = Object.fromEntries(
            Object.entries(data).filter(
                ([_, value]) =>
                    value !== undefined &&
                    value !== null &&
                    value !== '' &&
                    value !== 0 &&
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
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting recipes going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Получение отдельного рецепта
export const getRecipeByIdApi = async (id: string): Promise<Recipe> => {
    try {
        const response = await axios.get<Recipe>(
            `${apiUrl}api/receipts/${id}`,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );
        const data = response.data;
        const recipe: Recipe = {
            ...data,
            category:
                typeof data.category === 'string'
                    ? Category[data.category as keyof typeof Category]
                    : Category.hot,
            cookingDifficulty:
                typeof data.cookingDifficulty === 'string'
                    ? CookingDifficulty[
                          data.cookingDifficulty as keyof typeof CookingDifficulty
                      ]
                    : CookingDifficulty.easy,
            tags: Array.isArray(data.tags)
                ? (data.tags
                      .map((value) => Tag[value as keyof typeof Tag])
                      .filter(Boolean) as Tag[])
                : [],
            cookingSteps: Array.isArray(data.cookingSteps)
                ? data.cookingSteps.map(
                      (step: CookingStep) => step as CookingStep
                  )
                : [],
            ingredients: Array.isArray(data.ingredients)
                ? data.ingredients.map(
                      (ing: Ingredient) =>
                          ({
                              name: String(ing.name),
                              measure:
                                  typeof ing.measure === 'string'
                                      ? Measure[
                                            ing.measure as keyof typeof Measure
                                        ] || Measure.piece
                                      : Measure.piece,
                              numericValue: Number(ing.numericValue),
                          } as Ingredient)
                  )
                : [],
            user: data.user as ShortUser,
        };
        console.log(recipe);

        return recipe;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting recipe going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Получение списка рецептов пользователя
export const getRecipesByUserApi = async (data: {
    userId: string;
}): Promise<TRecipesResponse> => {
    try {
        const response = await axios.get<TRecipesResponse>(
            `${apiUrl}api/receipts`,
            {
                params: data,
                paramsSerializer: (params) =>
                    qs.stringify(params, { arrayFormat: 'repeat' }),
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting users recipes going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

export type RegisterUserData = {
    login: string;
    emailOrPhone: string;
    password: string;
};

//Регистрация пользователя
export const registerUserApi = async (
    data: RegisterUserData
): Promise<User> => {
    try {
        const response = await axios.post<User>(
            `${apiUrl}api/auth/register`,
            data,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Register failed');
        }
        throw new Error('Unknown error occurred');
    }
};

export type AuthUserData = {
    login: string;
    password: string;
    rememberMe?: boolean;
};

//Аутентификация пользователя
export const loginUserApi = async (data: AuthUserData): Promise<User> => {
    try {
        const response = await axios.post<User>(
            `${apiUrl}api/auth/login`,
            { adminEntrance: false, rememberMe: true, ...data },
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Login failed');
        }
        throw new Error('Unknown error occurred');
    }
};

//Проверка авторизации пользователя
export const checkUserApi = async (): Promise<User> => {
    try {
        const response = await axios.post<User>(
            `${apiUrl}api/auth/check-in`,
            {},
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Check-in failed');
        }
        throw new Error('Unknown error occurred');
    }
};

//Выход пользователя
export const logoutUserApi = async (): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/auth/logout`,
            {},
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Logout failed');
        }
        throw new Error('Unknown error occurred');
    }
};

export type TCollectionsData = {
    Page: number;
    Limit?: number;
    Title?: string;
};

export type TCollectionsResponse = {
    data: ShortCollection[];
    entitiesCount: number;
    pagesCount: number;
};

//Получение списка всех коллекций
export const getCollectionsApi = async (
    data: TCollectionsData
): Promise<TCollectionsResponse> => {
    try {
        const params = Object.fromEntries(
            Object.entries(data).filter(
                ([_, value]) =>
                    value !== undefined &&
                    value !== null &&
                    value !== '' &&
                    value !== 0 &&
                    (!Array.isArray(value) || value.length > 0)
            )
        );

        const response = await axios.get<TCollectionsResponse>(
            `${apiUrl}api/receipt-collections/all/by-filter`,
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
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting recipes collections going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Получение отдельной коллекции
export const getCollectionByIdApi = async (id: string): Promise<Collection> => {
    try {
        const response = await axios.get<Collection>(
            `${apiUrl}api/receipt-collections/${id}`,
            {
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );
        const data = response.data;
        const collection: Collection = {
            ...data,
            receipts: data.receipts.map((receipt) => {
                return {
                    ...receipt,
                    category:
                        typeof receipt.category === 'string'
                            ? Category[
                                  receipt.category as keyof typeof Category
                              ]
                            : Category.hot,
                    cookingDifficulty:
                        typeof receipt.cookingDifficulty === 'string'
                            ? CookingDifficulty[
                                  receipt.cookingDifficulty as keyof typeof CookingDifficulty
                              ]
                            : CookingDifficulty.easy,
                    tags: Array.isArray(receipt.tags)
                        ? (receipt.tags
                              .map((value) => Tag[value as keyof typeof Tag])
                              .filter(Boolean) as Tag[])
                        : [],
                };
            }),
        };

        return collection;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting collection going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

//Получение списка коллекций у пользователя
export const getCollectionsByUserApi = async (data: {
    userId: string;
}): Promise<TCollectionsResponse> => {
    try {
        const response = await axios.get<TCollectionsResponse>(
            `${apiUrl}api/receipt-collections/all/by-filter`,
            {
                params: data,
                paramsSerializer: (params) =>
                    qs.stringify(params, { arrayFormat: 'repeat' }),
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Getting recipes collections going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};

// Удаление подборки рецептов
export const deleteCollectionApi = async (id: string): Promise<string> => {
    try {
        const response = await axios.delete(
            `${apiUrl}api/receipt-collections/${id}`,
            {
                headers: {
                    Accept: '*/*',
                },
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Deleting collection failed');
        }

        throw new Error('Unknown error occurred');
    }
};
