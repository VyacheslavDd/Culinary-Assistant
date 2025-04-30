import { Recipe } from "./recipe.type";
import { SelectionRecipes } from "./selection-of-recipes.type";

export type User = {
    id: string;
    login: string;
    email: string;
    phone: string;
    pictureUrl: string;
    // recipeHistory: Recipe[];
    // recipes: Recipe[];
    // selectionRecipes: SelectionRecipes[];
};