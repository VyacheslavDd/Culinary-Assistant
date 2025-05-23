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
import { getEnumValueByString } from 'utils/transform';
import { API_URL } from 'utils/variables';

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

const apiUrl = API_URL || 'http://localhost:5000/';

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
                    ? getEnumValueByString(Category, data.category) ??
                      Category.mainCourse
                    : Category.mainCourse,

            cookingDifficulty:
                typeof data.cookingDifficulty === 'string'
                    ? getEnumValueByString(
                          CookingDifficulty,
                          data.cookingDifficulty
                      ) ?? CookingDifficulty.easy
                    : CookingDifficulty.easy,

            tags: Array.isArray(data.tags)
                ? (data.tags
                      .map((tag) => getEnumValueByString(Tag, tag))
                      .filter(Boolean) as Tag[])
                : [],

            cookingSteps: Array.isArray(data.cookingSteps)
                ? data.cookingSteps.map((step) => step as CookingStep)
                : [],

            ingredients: Array.isArray(data.ingredients)
                ? data.ingredients.map((ing) => ({
                      name: String(ing.name),
                      measure:
                          typeof ing.measure === 'string'
                              ? getEnumValueByString(Measure, ing.measure) ??
                                Measure.piece
                              : Measure.piece,
                      numericValue: Number(ing.numericValue),
                  }))
                : [],

            user: data.user as ShortUser,
        };

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
            withCredentials: true,
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

// Загрузка фотографии рецепта
export const uploadRecipeImage = async (file?: File): Promise<string> => {
    try {
        if (!file) {
            return '';
        }

        const formData = new FormData();
        formData.append('Files', file);

        const response = await axios.post<{ url: string }[]>(
            `${apiUrl}api/files/receipts`,
            formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Accept: '*/*',
                },
                withCredentials: true,
            }
        );

        return response.data[0].url;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            if (typeof error.response?.data === 'string') {
                throw new Error(error.response.data);
            }

            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }

            throw new Error('Image upload failed');
        }

        throw new Error('Unknown error occurred during image upload');
    }
};

export interface CreateRecipeDto {
    title: string;
    description: string;
    tags: string[];
    category: string;
    cookingDifficulty: string;
    cookingTime: number;
    ingredients: Ingredient[];
    cookingSteps: CookingStep[];
    picturesUrls: {
        url: string;
    }[];
    userId: string;
}

// Добавление нового рецепта
export const createRecipe = async (
    recipe: CreateRecipeDto,
    imageFile?: File
): Promise<string> => {
    try {
        const imageUrl = await uploadRecipeImage(imageFile);

        let recipeWithImage: CreateRecipeDto = {
            ...recipe,
        };

        if (imageUrl) {
            recipeWithImage = {
                ...recipeWithImage,
                picturesUrls: [{ url: imageUrl }],
            };
        }

        const response = await axios.post<string>(
            `${apiUrl}api/receipts`,
            recipeWithImage,
            {
                headers: {
                    'Content-Type': 'application/json-patch+json',
                    Accept: '*/*',
                },
                withCredentials: true,
            }
        );

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Recipe creation failed'
            );
        }
        throw new Error('Unknown error during recipe creation');
    }
};

export interface UpdateRecipeDto {
    title?: string;
    description?: string;
    tags?: string[];
    category?: string;
    cookingDifficulty?: string;
    cookingTime?: number;
    ingredients?: Ingredient[];
    cookingSteps?: CookingStep[];
    picturesUrls?: {
        url: string;
    }[];
    userId: string;
}

// Редактировать рецепт
export const updateRecipe = async (
    id: string,
    updatedRecipe: UpdateRecipeDto,
    imageFile?: File
): Promise<void> => {
    try {
        let picturesUrls = updatedRecipe.picturesUrls;

        // Если выбрано новое изображение — загружаем и подменяем URL
        if (imageFile) {
            const uploadedUrl = await uploadRecipeImage(imageFile);
            picturesUrls = uploadedUrl ? [{ url: uploadedUrl }] : picturesUrls;
        }

        const payload: UpdateRecipeDto = {
            ...updatedRecipe,
            picturesUrls,
        };

        await axios.put(
            `${apiUrl}api/receipts/${id}`,
            payload,
            {
                headers: {
                    'Content-Type': 'application/json-patch+json',
                    Accept: '*/*',
                },
                withCredentials: true,
            }
        );
    } catch (error) {
        if (axios.isAxiosError(error)) {
            throw new Error(
                error.response?.data?.message || 'Failed to update recipe'
            );
        }
        throw new Error('Unknown error during recipe update');
    }
};

// Поставить оценку рецепту
export const putRateRecipeApi = async (
    id: string,
    rate: number
): Promise<void> => {
    try {
        await axios.post(
            `${apiUrl}api/receipts/${id}/rate`,
            { rate: rate },
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

            throw new Error('Putting rate get failed');
        }

        throw new Error('Unknown error occurred');
    }
};

export type rateType = {
    rate: number;
};

//Получение оценки пользователя рецепта
export const getRecipeRateApi = async (id: string): Promise<rateType> => {
    try {
        const response = await axios.get<rateType>(
            `${apiUrl}api/receipts/${id}/rate`,
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

            throw new Error('Getting rate of recipe going wrong');
        }
        throw new Error('Unknown error occurred');
    }
};
