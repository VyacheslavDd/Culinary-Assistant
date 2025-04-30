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

            throw new Error('Login failed');
        }
        throw new Error('Unknown error occurred');
    }
};

// export type TRecipeResponse = {
//     data: ShortRecipe;
//     entitiesCount?: number;
//     pagesCount?: number;
// };

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
                ? data.cookingSteps.map((step: any) => step as CookingStep)
                : [],
            ingredients: Array.isArray(data.ingredients)
                ? data.ingredients.map(
                      (ing: any) =>
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

            throw new Error('Login failed');
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
            { adminEntrance: false, rememberMe: false, ...data },
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
