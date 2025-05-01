import { FilterShort } from 'components/common/filter/filter';
import styles from './my-recipes.module.scss';
import { Preloader } from 'components/preloader';
import { Catalog } from 'components/catalog/catalog';
import { useSelector } from 'store/store';
import { selectUserLoading, selectUserRecipes } from 'store/user.slice';
import { useMemo, useState } from 'react';
import ScrollToTop from 'components/common/scrollToTop';

function MyRecipesPage() {
    const isLoading = useSelector(selectUserLoading);
    const recipes = useSelector(selectUserRecipes);

    const [searchTerm, setSearchTerm] = useState('');

    const filteredRecipes = useMemo(() => {
        const lowerSearch = searchTerm.toLowerCase();
        return recipes.filter((recipe) =>
            recipe.title.toLowerCase().includes(lowerSearch)
        );
    }, [recipes, searchTerm]);

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(e.target.value);
    };

    return (
        <>
            <ScrollToTop />
            <main className={styles.main}>
                <h1 className={styles.title}>Мои рецепты</h1>
                <FilterShort
                    onChange={handleSearchChange}
                    value={searchTerm}
                    isFindShows={false}
                />
                {isLoading ? (
                    <Preloader />
                ) : (
                    <Catalog recipes={filteredRecipes} />
                )}
            </main>
        </>
    );
}

export default MyRecipesPage;
