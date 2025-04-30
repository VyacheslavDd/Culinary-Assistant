import { Category, CookingDifficulty, Ingredient, Tag } from 'types';

export type Filter = {
    SearchByTitle?: string;
    // SearchByIngredients: Ingredient[];
    SearchByIngredients: string;
    CookingTimeFrom?: number;
    CookingTimeTo?: number;
    CookingDifficulties?: CookingDifficulty[];
    Categories?: Category[];
    Tags?: Tag[];
};
