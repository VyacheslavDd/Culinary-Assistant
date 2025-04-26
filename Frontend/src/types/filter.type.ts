import { Category, CookingDifficulty, Ingredient, Tag } from 'types';

export type Filter = {
    SearchByTitle: string;
    SearchByIngredients: Ingredient[];
    CookingTimeFrom: number;
    CookingTimeTo: number;
    CookingDifficulty?: CookingDifficulty[];
    Category: Category[];
    Tags: Tag[];
};
