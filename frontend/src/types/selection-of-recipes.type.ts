import { ShortRecipe } from './short-recipe.type';
import { ShortUser } from './short-user.type';

export type SelectionRecipes = {
    id: string;
    title: string;
    coverImage: string[];
    recipes: ShortRecipe[];
    isPrivate: boolean;
    isFavourite: boolean;
    author: ShortUser;
}
