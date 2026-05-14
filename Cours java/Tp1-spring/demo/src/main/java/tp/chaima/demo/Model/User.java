package tp.chaima.demo.model;

import jakarta.persistence.*;
import lombok.*;
import java.util.List;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
@Entity
@Table(name = "user")
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;
    private String email;

    // Relation One to Many avec Classe
    @ManyToOne
    @JoinColumn(name = "classe_id")
    private Classe classe;

    @ManyToMany(mappedBy = "users")
    private List<Prof> profs;
}