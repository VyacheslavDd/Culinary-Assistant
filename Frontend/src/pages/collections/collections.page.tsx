import { Best, CollList } from 'components/collections';
import styles from './collections.module.scss';

function CollectionsPage() {
    return (
        <div className={styles.mainContainer}>
            <Best />
            <CollList title='Все подборки' />
        </div>
    );
}

export default CollectionsPage;
