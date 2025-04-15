import { ButtonsReceipt, Ingredients, MainInfo, Ratings, Review, Reviews, Steps } from 'components/receipt';
import styles from './receipt.module.scss';

function ReceiptPage() {
    return (
        <div className={styles.background}>
            <div className={styles.mainContainer}>
                <div className={styles.backContainer}>
                    <a href='/' className={styles.back}>
                        <span className={styles.arrow}></span>
                        На главную
                    </a>
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
    );
}

export default ReceiptPage;
