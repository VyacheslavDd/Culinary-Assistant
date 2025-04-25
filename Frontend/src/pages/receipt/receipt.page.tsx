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
import { useNavigate } from 'react-router-dom';
import ScrollToTop from 'components/common/scrollToTop';

function ReceiptPage() {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate(-1);
    };

    return (
        <>
            <ScrollToTop />{' '}
            <div className={styles.background}>
                <div className={styles.mainContainer}>
                    <div className={styles.backContainer}>
                        <button className={styles.back} onClick={handleClick}>
                            <span className={styles.arrow}></span>Назад
                        </button>
                    </div>
                    <div className={styles.container}>
                        <MainInfo />
                        <div className={styles.infoContainer}>
                            <Steps />
                            <Ingredients />
                        </div>
                        <div className={styles.reviewContainer}>
                            <ButtonsReceipt />
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
