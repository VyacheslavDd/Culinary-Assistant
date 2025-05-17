import styles from './catalog.module.scss';
import { CatalogCard } from './card/card';
// import { arrayRecipesMock } from 'mocks/catalog.mock';
import { ShortRecipe } from 'types';

type CatalogProps = {
    recipes: ShortRecipe[];
    limit?: number;
    isEdit?: boolean;
    onDelete?: (id: string) => void;
};

export function Catalog(props: CatalogProps) {
    const { limit, recipes, isEdit = false, onDelete} = props;
    // const recipes = arrayRecipesMock;
    const limitedRecipes = limit ? recipes.slice(0, props.limit) : recipes;

    return (
        <div className={styles.catalog}>
            {limitedRecipes.length === 0 ? (
                <p>Ничего не найдено</p>
            ) : (
                limitedRecipes.map((recipe) => (
                    <CatalogCard key={recipe.id} recipe={recipe} isEdit={isEdit} onDelete={onDelete} />
                ))
            )}
            {/* {limitedRecipes.map((recipe) => (
                <CatalogCard key={recipe.id} recipe={recipe} />
            ))} */}
        </div>
    );
}
