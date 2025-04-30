import { Category } from './category.enum';
import { CookingDifficulty } from './cookingDifficulty.enum';
import { Ingredient } from './ingredient.type';
import { ShortUser } from './short-user.type';
import { Tag } from './tags.enum';

export type Recipe = {
    id: string;
    title: string;
    description: string;
    ingredients: Ingredient[];
    cookingSteps: CookingStep[];
    calories: number;
    cookingTime: number;
    proteins: number;
    fats: number;
    carbohydrates: number;
    tags: Tag[];
    category: Category;
    cookingDifficulty: CookingDifficulty;
    popularity: number;
    rating: number;
    mainPictureUrl: string;
    picturesUrls: { url: string }[];
    user: ShortUser;
};

export type CookingStep = {
    step: number;
    title: string;
    description: string;
};
