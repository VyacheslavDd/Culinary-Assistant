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
} from 'types';

export type TRecipesData = {
    Page: number;
    Limit?: number;
    Tags?: Tag[];
    SearchByTitle?: string;
    SearchByIngredients?: string[];
    StrictIngredientsSearch: boolean;
    SortOption: 'byCookingTime' | 'byPopularity' | 'byCalories';
    IsAscendingSorting: boolean;
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
                withCredentials: true,
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

// Поставить лайк рецепту
export const likeRecipeApi = async (id: string): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/receipts/${id}/likes`,
            {},
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Adding to favorites failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Добавить рецепт в избранное
export const favoriteRecipeApi = async (id: string): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/receipts/${id}/favourite`,
            {},
            {
                headers: {
                    Accept: '*/*',
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

            throw new Error('Adding to favorites failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Удалить из избранного
export const unfavouriteRecipeApi = async (id: string): Promise<void> => {
    try {
        await axios.delete(`${apiUrl}api/receipts/${id}/unfavourite`, {
            headers: {
                Accept: '*/*',
            },
            withCredentials: true,
        });
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Removing from favorites failed');
        }

        throw new Error('Unknown error occurred');
    }
};

// Удаление рецепта
export const deleteRecipeApi = async (id: string): Promise<string> => {
    try {
        const response = await axios.delete(`${apiUrl}api/receipt/${id}`, {
            headers: {
                Accept: '*/*',
            },
        });

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

            throw new Error('Getting users recipes going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};
