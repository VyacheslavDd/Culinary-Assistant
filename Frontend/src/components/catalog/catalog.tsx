import styles from './catalog.module.scss';
import { CatalogCard } from './card/card';
import { arrayRecipesMock } from 'mocks/catalog.mock';

type CatalogProps = {
    limit?: number;
};

export function Catalog(props: CatalogProps) {
    const { limit } = props;
    const recipes = arrayRecipesMock;
    const limitedRecipes = limit ? recipes.slice(0, props.limit) : recipes;

    return (
        <div className={styles.catalog}>
            {limitedRecipes.map((recipe) => (
                <CatalogCard key={recipe.id} recipe={recipe} />
            ))}
        </div>
    );
}
