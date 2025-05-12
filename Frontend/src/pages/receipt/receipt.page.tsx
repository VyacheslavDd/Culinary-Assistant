import {
    ButtonsReceipt,
    Ingredients,
    MainInfo,
    Ratings,
    Review,
    Reviews,
    Steps,
} from 'components/receipt';
import styles from './receipt.module.scss';
import { useNavigate, useParams } from 'react-router-dom';
import ScrollToTop from 'components/common/scrollToTop';
import { useEffect, useState } from 'react';
import { Recipe } from 'types';
import { getRecipeByIdApi } from 'store/api';
import { Preloader } from 'components/preloader';

function ReceiptPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [recipe, setRecipe] = useState<Recipe | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchRecipe = async () => {
            try {
                if (!id) {
                    throw new Error('Recipe ID is missing');
                }
                const data = await getRecipeByIdApi(id);
                setRecipe(data);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : 'Unknown error occurred'
                );
            } finally {
                setLoading(false);
            }
        };

        fetchRecipe();
    }, [id]);

    const handleClick = () => {
        navigate(-1);
    };

    if (loading) {
        return <Preloader />;
    }

    if (error) {
        return <div className={styles.error}>Error: {error}</div>;
    }

    if (!recipe) {
        return <div className={styles.error}>Recipe not found</div>;
    }

    return (
        <>
            <ScrollToTop />
            <div className={styles.background}>
                <div className={styles.mainContainer}>
                    <div className={styles.backContainer}>
                        <button className={styles.back} onClick={handleClick}>
                            <span className={styles.arrow}></span>Назад
                        </button>
                    </div>
                    <div className={styles.container}>
                        <MainInfo recipe={recipe} />
                        <div className={styles.infoContainer}>
                            <Steps steps={recipe.cookingSteps} />
                            <Ingredients ingredients={recipe.ingredients} />
                        </div>
                        <div className={styles.reviewContainer}>
                            <ButtonsReceipt name={recipe.title} />
                            <Ratings />
                            <Review />
                            <Reviews />
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}

export default ReceiptPage;
